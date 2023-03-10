using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    [SerializeField] private LayerMask draggableLayer;
    [SerializeField] private LayerMask gridObjectLayer;
    [SerializeField] private float gridSize = 0.1f;
    private Transform _draggableObject;
    private Character character;
    private Vector3 _startDraggablePosition;
    private float _dragDistance;
    private Vector3 _offset;
    private Camera _mainCamera;
    private LevelGrid levelGrid;
    private InventoryGrid inventoryGrid;


    public Unit[] freeUnits;
    public Unit[] allUnits;


    public enum DragState
    {
        Inv2Inv,
        Inv2Grid,
        Grid2Inv,
        Grid2Grid
    }

    [SerializeField] private DragState dragState = DragState.Inv2Inv;

    private void Start()
    {
        levelGrid = LevelGrid.Instance;
        inventoryGrid = InventoryGrid.Instance;
        _mainCamera = Camera.main;

    }

    private void Update()
    {
        if (Input.touchCount != 1)
        {
            StopDragging();
            return;
        }

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            StartDragging(touch);
        }

        else if (_draggableObject && touch.phase == TouchPhase.Moved)
        {
            character.GetAnimator.SetBool("fall", true);
            MoveDraggableObject(touch);
        }

        else if (_draggableObject && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
        {
            character.GetAnimator.SetBool("fall", false);
            StopDragging();
        }
    }

    private void StartDragging(Touch touch)
    {
        Ray ray = _mainCamera.ScreenPointToRay(touch.position);
        if (Physics.Raycast(ray, out RaycastHit hit) && IsLayer(hit.collider.gameObject.layer, draggableLayer))
        {
            _draggableObject = hit.transform;

            character = new Character(
                _draggableObject, 
                hit.transform.GetComponent<Collider>(), 
                hit.transform.GetComponent<Animator>(),
                hit.transform.GetComponent<Unit>()
                );


            _startDraggablePosition = _draggableObject.position;
            _dragDistance = character.GetTransform.position.z - _mainCamera.transform.position.z;
            Vector3 position = new Vector3(touch.position.x, touch.position.y, _dragDistance);
            position = _mainCamera.WorldToScreenPoint(character.GetTransform.position);
            _offset = character.GetTransform.position - _mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, position.z));
        }
    }
    private Transform lastfloor;
    private void MoveDraggableObject(Touch touch)
    {
        Ray ray = _mainCamera.ScreenPointToRay(touch.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, gridObjectLayer))
        {
            float x = Mathf.Floor(hit.point.x / gridSize) * gridSize;
            float y = Mathf.Floor(hit.point.y / gridSize) * gridSize + 2;
            float z = Mathf.Floor(hit.point.z / gridSize) * gridSize;

            Vector3 cursorPosition = new Vector3(x, y, z) + _offset;
            character.GetTransform.position = cursorPosition;
            lastfloor = hit.transform;
        }
        character.GetCollider.enabled = false;
    }

    private void StopDragging()
    {
        if (_draggableObject)
        {
            if(!lastfloor) 
                character.GetTransform.position = _startDraggablePosition;
            else
            {
                CharacterDragging();
            }

            character.GetCollider.enabled = true;
            character = null;
            _draggableObject = null;
        }
    }

    private void CharacterDragging()
    {
        InventoryGrid inventoryGrid = InventoryGrid.Instance;
        GridPosition lastgridPosition;
        GridPosition gridPosition;

        if (!character.GetUnit.OnGrid)
        {
            lastgridPosition = inventoryGrid.GetInventoryPosition(_startDraggablePosition);
        }
        else
        {
            lastgridPosition = levelGrid.GetGridPosition(_startDraggablePosition);
        }
        if (lastfloor.GetComponent<GridSystemVisualSingle>().isInventory)
        {
            gridPosition = inventoryGrid.GetInventoryPosition(lastfloor.position);
        }
        else
        {
            gridPosition = levelGrid.GetGridPosition(lastfloor.position);
        }
        CalculateState();

        DragUnit(lastgridPosition, gridPosition);
    }

    private void DragUnit(GridPosition lastgridPosition, GridPosition gridPosition)
    {

        Debug.Log($"{character.GetUnit.GetUnitName} moving from {lastgridPosition} to {gridPosition} at dragstate {dragState.ToString()}");
        switch (dragState)
        {
            case DragState.Grid2Grid:
                {
                    if (!levelGrid.HasAnyUnitOnGridPosition(gridPosition))
                    {
                        Unit unit = character.GetUnit;
                        levelGrid.UnitMovedGridPosition(unit, lastgridPosition, gridPosition);

                        unit.Move(levelGrid.GetWorldPosition(gridPosition));
                        levelGrid.RemoveAnyUnitAtGridPosition(lastgridPosition);
                    }
                    else
                    {
                        levelGrid.UnitSwappedGridPosition(character.GetUnit, levelGrid.GetUnitAtGridPosition(gridPosition), gridPosition, lastgridPosition);
                    }

                    break;
                }

            case DragState.Inv2Inv:
                {
                    if (!inventoryGrid.HasAnyUnitOnInventoryPosition(gridPosition))
                    {
                        Unit unit = character.GetUnit;
                        inventoryGrid.UnitMovedInventoryPosition(unit, lastgridPosition, gridPosition);

                        unit.Move(inventoryGrid.GetInventoryWorldPosition(gridPosition));
                        inventoryGrid.RemoveAnyUnitAtInventoryPosition(lastgridPosition);
                    }
                    else
                    {
                        inventoryGrid.UnitSwappedInventoryPosition(character.GetUnit, inventoryGrid.GetUnitAtInventoryPosition(gridPosition), gridPosition, lastgridPosition);
                    }

                    break;
                }

            case DragState.Inv2Grid:
                {
                    if (!levelGrid.HasAnyUnitOnGridPosition(gridPosition))
                    {
                        Unit unit = character.GetUnit;

                        unit.Move(levelGrid.GetWorldPosition(gridPosition));
                        inventoryGrid.RemoveAnyUnitAtInventoryPosition(lastgridPosition);

                        levelGrid.AddUnitAtGridPosition(gridPosition, unit);
                        unit.OnGrid = true;
                    }
                    else
                    {
                        Unit unitA = character.GetUnit;
                        Unit unitB = levelGrid.GetUnitAtGridPosition(gridPosition);

                        inventoryGrid.RemoveAnyUnitAtInventoryPosition(lastgridPosition);
                        levelGrid.RemoveAnyUnitAtGridPosition(gridPosition);

                        levelGrid.AddUnitAtGridPosition(gridPosition, unitA);
                        unitA.Move(levelGrid.GetWorldPosition(gridPosition));
                        unitA.OnGrid = true;

                        inventoryGrid.AddUnitAtInventoryPosition(lastgridPosition, unitB);
                        unitB.Move(inventoryGrid.GetInventoryWorldPosition(lastgridPosition));
                        unitB.OnGrid = false;
                    }

                    break;
                }

            case DragState.Grid2Inv:
                {
                    if (!inventoryGrid.HasAnyUnitOnInventoryPosition(gridPosition))
                    {
                        Unit unit = character.GetUnit;

                        unit.Move(inventoryGrid.GetInventoryWorldPosition(gridPosition));
                        levelGrid.RemoveAnyUnitAtGridPosition(lastgridPosition);

                        inventoryGrid.AddUnitAtInventoryPosition(gridPosition, unit);
                        unit.OnGrid = false;
                    }
                    else
                    {
                        Unit unitA = character.GetUnit;
                        Unit unitB = inventoryGrid.GetUnitAtInventoryPosition(gridPosition);

                        inventoryGrid.RemoveAnyUnitAtInventoryPosition(gridPosition);
                        levelGrid.RemoveAnyUnitAtGridPosition(lastgridPosition);

                        inventoryGrid.AddUnitAtInventoryPosition(lastgridPosition, unitA);
                        unitA.Move(inventoryGrid.GetInventoryWorldPosition(lastgridPosition));
                        unitA.OnGrid = false;

                        levelGrid.AddUnitAtGridPosition(gridPosition, unitB);
                        unitB.Move(levelGrid.GetWorldPosition(gridPosition));
                        unitB.OnGrid = true;

                    }

                    break;
                }
        }
    }

    private void CalculateState()
    {
        switch ((!character.GetUnit.OnGrid, lastfloor.GetComponent<GridSystemVisualSingle>().isInventory))
        {
            case (true, true):
                dragState = DragState.Inv2Inv;
                break;
            case (true, false):
                dragState = DragState.Inv2Grid;
                break;
            case (false, true):
                dragState = DragState.Grid2Inv;
                break;
            default:
                dragState = DragState.Grid2Grid;
                break;
        }
    }
    private bool IsLayer(int gameObjectLayer, LayerMask layer)
    {
        return ((1 << gameObjectLayer) & layer) != 0;
    }
}