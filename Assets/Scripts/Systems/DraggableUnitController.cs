using System.Collections;
using UnityEngine;

public class DraggableUnitController : MonoBehaviour
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

    [SerializeField] private GameObject sellUI;
    [SerializeField] private GameObject marketUI;

    [SerializeField] private DragState dragState = DragState.Inv2Inv;


    private bool selling;

    public void SetSelling(bool value)
    {
        if (GameManager.Instance.GetAllUnits.Count > 1)
        {
            selling = value;
        }
        else {
            selling = false;
        }
    }

    private GridPosition lastgridPosition;
    private GridPosition gridPosition;
    private void Awake()
    {
        Input.simulateMouseWithTouches = true;
    }
    private void Start()
    {
        levelGrid = LevelGrid.Instance;
        inventoryGrid = InventoryGrid.Instance;
        _mainCamera = Camera.main;

    }



    private void Update()
    {
        SwitchTouchMethod();

    }

    private void SwitchTouchMethod()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            TouchAndroid();
        }
        else
        {
            TouchMouse();
        }
    }

    private void TouchMouse()
    {
        Vector2 touchPosition = Input.mousePosition;
        if (Input.GetMouseButtonDown(0))
        {
            StartDragging(touchPosition);
        }
        else if (_draggableObject && Input.GetMouseButton(0))
        {
            MoveDraggableObject(touchPosition);
        }
        else if (_draggableObject && Input.GetMouseButtonUp(0))
        {
            if (selling)
            {
                DropToSell();
            }
            else
            {
                DropWithOutSell();
            }
        }
    }
    private void TouchAndroid()
    {
        if (Input.touchCount != 1)
        {
            StopDragging();
            return;
        }

        Touch touch = Input.GetTouch(0);
        TouchPhase touchPhase = touch.phase;
        Vector2 touchPosition = touch.position;

        if (touchPhase == TouchPhase.Began)
        {
            StartDragging(touchPosition);
        }
        else if (_draggableObject && touchPhase == TouchPhase.Moved)
        {

            MoveDraggableObject(touchPosition);

        }
        else if (_draggableObject && touchPhase == TouchPhase.Ended)
        {
            if (selling)
            {
                DropToSell();
            }
            else if (!selling)
            {
                DropWithOutSell();
            }
        }
        else if (_draggableObject && touchPhase == TouchPhase.Canceled)
        {
            DropWithOutSell();
        }
    }

    private void DropWithOutSell()
    {
        StopDragging();
        ResetUI();
    }

    private void DropToSell()
    {
        SellUnit();
        ResetUI();
    }
    [SerializeField] private GameObject eventManager;

    private IEnumerator ReActivate() {
        eventManager.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        eventManager.SetActive(true);
    }

    private void MoveDraggableObject(Vector2 touchPosition)
    {
        character.GetUnit.charState = CharState.Fall;
        Ray ray = _mainCamera.ScreenPointToRay(touchPosition);
        RaycastHit hit;
        GameState stateIndex = GameStateSystem.Instance.GetCurrentState;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, gridObjectLayer))
        {
            if (stateIndex.IsCombatState && character.GetUnit.OnGrid)
            {
                StartCoroutine(nameof(ReActivate));
                //_draggableObject = null;
               // character.GetUnit.charState = CharState.Idle;
                return;
            }
            float x = Mathf.Floor(hit.point.x / gridSize) * gridSize;
            float y = Mathf.Floor(hit.point.y / gridSize) * gridSize + 1.5f;
            float z = Mathf.Floor(hit.point.z / gridSize) * gridSize;

            Vector3 cursorPosition = new Vector3(x, y, z) + _offset;
            character.GetTransform.position = cursorPosition;
            lastfloor = hit.transform;
        }
        character.GetCollider.enabled = false;
    }
    private void StartDragging(Vector2 touchPosition)
    {
        Ray ray = _mainCamera.ScreenPointToRay(touchPosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && IsLayer(hit.collider.gameObject.layer, draggableLayer))
        {
            _draggableObject = hit.transform;
            character =
                new Character(
                    _draggableObject,
                    hit.transform.GetComponent<Collider>(),
                    hit.transform.GetComponent<Unit>()
                );

            GameState stateIndex = GameStateSystem.Instance.GetCurrentState;
            if (stateIndex.IsCombatState && character.GetUnit.OnGrid) {
                StartCoroutine(nameof(ReActivate));
                // _draggableObject = null;
                return;
            }

            sellUI.SetActive(true);
            marketUI.SetActive(false);

            _startDraggablePosition = _draggableObject.position;
            _dragDistance = character.GetTransform.position.z - _mainCamera.transform.position.z;
            Vector3 position = new Vector3(touchPosition.x, touchPosition.y, _dragDistance);
            position = _mainCamera.WorldToScreenPoint(character.GetTransform.position);
            _offset = character.GetTransform.position - _mainCamera.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, position.z));
        }
    }

    private void SellUnit()
    {
        Unit unit = character.GetUnit;
        Economy.SellUnit(unit);

        if (unit.OnGrid)
        {
            levelGrid.RemoveUnitAtGridPosition(levelGrid.GetGridPosition(_startDraggablePosition), unit);
        }
        else
        {
            inventoryGrid.RemoveUnitAtInventoryPosition(inventoryGrid.GetInventoryPosition(_startDraggablePosition), unit);
        }
        Destroy(character.GetTransform.gameObject);
        selling = false;
    }

    private void ResetUI()
    {
        sellUI.SetActive(false);
        marketUI.SetActive(true);
    }


    private Transform lastfloor;


    private void StopDragging()
    {

        if (_draggableObject)
        {
            character.GetUnit.charState = CharState.Idle;
            if (!lastfloor)
                character.GetTransform.position = _startDraggablePosition;
            else
            {
                CharacterDragging();
            }

            character.GetCollider.enabled = true;
            character = default(Character);
            _draggableObject = null;
        }
    }

    private void CharacterDragging()
    {
        GameState stateIndex = GameStateSystem.Instance.GetCurrentState;
        if (stateIndex.IsCombatState && character.GetUnit.OnGrid)
        {
            _draggableObject = null;
            character.GetUnit.charState = CharState.Idle;
            return;
        }

        InventoryGrid inventoryGrid = InventoryGrid.Instance;


        if (character.GetUnit.OnGrid)
        {
            lastgridPosition = levelGrid.GetGridPosition(_startDraggablePosition);
        }
        else
        {
            lastgridPosition = inventoryGrid.GetInventoryPosition(_startDraggablePosition);
        }

        GridSystemVisualSingle floorGridSystemVisual = lastfloor.GetComponent<GridSystemVisualSingle>();
        gridPosition = floorGridSystemVisual.isInventory
            ? inventoryGrid.GetInventoryPosition(lastfloor.position)
            : levelGrid.GetGridPosition(lastfloor.position);

        // CalculateState();
        DragUnit(lastgridPosition, gridPosition);
    }



    private void DragUnit(GridPosition lastgridPosition, GridPosition gridPosition)
    {

        switch (GetDragState())
        {
            case DragState.Grid2Grid:
                {
                    if (GameStateSystem.Instance.GetCurrentState.IsCombatState) return;
                    if (!levelGrid.HasAnyUnitOnGridPosition(gridPosition))
                    {
                        Unit unit = character.GetUnit;
                        levelGrid.UnitMovedGridPosition(unit, lastgridPosition, gridPosition);

                        unit.TeleportToPosition(levelGrid.GetWorldPosition(gridPosition), gridPosition);
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

                        unit.TeleportToPosition(inventoryGrid.GetInventoryWorldPosition(gridPosition), gridPosition);
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
                    if (GameStateSystem.Instance.GetCurrentState.IsCombatState) return;
                    if (!levelGrid.HasAnyUnitOnGridPosition(gridPosition))
                    {
                        Unit unit = character.GetUnit;

                        unit.TeleportToPosition(levelGrid.GetWorldPosition(gridPosition), gridPosition);
                        inventoryGrid.RemoveAnyUnitAtInventoryPosition(lastgridPosition);

                        levelGrid.AddUnitAtGridPosition(gridPosition, unit);
                        unit.OnGrid = true;

                        if (!GameManager.Instance.GridisFree())
                        {
                            unit.TeleportToPosition(inventoryGrid.GetInventoryWorldPosition(lastgridPosition), lastgridPosition);
                            levelGrid.RemoveAnyUnitAtGridPosition(gridPosition);

                            inventoryGrid.AddUnitAtInventoryPosition(lastgridPosition, unit);
                            unit.OnGrid = false;

                        }

                    }
                    else
                    {
                        Unit unitA = character.GetUnit;
                        Unit unitB = levelGrid.GetUnitAtGridPosition(gridPosition);

                        inventoryGrid.RemoveAnyUnitAtInventoryPosition(lastgridPosition);
                        levelGrid.RemoveAnyUnitAtGridPosition(gridPosition);

                        levelGrid.AddUnitAtGridPosition(gridPosition, unitA);
                        unitA.TeleportToPosition(levelGrid.GetWorldPosition(gridPosition), gridPosition);
                        unitA.OnGrid = true;

                        inventoryGrid.AddUnitAtInventoryPosition(lastgridPosition, unitB);
                        unitB.TeleportToPosition(inventoryGrid.GetInventoryWorldPosition(lastgridPosition), gridPosition);
                        unitB.OnGrid = false;
                    }

                    break;
                }

            case DragState.Grid2Inv:
                {
                    if (!inventoryGrid.HasAnyUnitOnInventoryPosition(gridPosition))
                    {
                        Unit unit = character.GetUnit;

                        unit.TeleportToPosition(inventoryGrid.GetInventoryWorldPosition(gridPosition), gridPosition);
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
                        unitA.TeleportToPosition(inventoryGrid.GetInventoryWorldPosition(lastgridPosition), gridPosition);
                        unitA.OnGrid = false;

                        levelGrid.AddUnitAtGridPosition(gridPosition, unitB);
                        unitB.TeleportToPosition(levelGrid.GetWorldPosition(gridPosition), gridPosition);
                        unitB.OnGrid = true;

                    }

                    break;
                }

            case DragState.Inv2Shop:
                break;
            case DragState.Grid2Shop:
                break;
        }
    }

    private DragState GetDragState()
    {
        return dragState = (!character.GetUnit.OnGrid, lastfloor.GetComponent<GridSystemVisualSingle>().isInventory) switch
        {
            (true, true) => DragState.Inv2Inv,
            (true, false) => DragState.Inv2Grid,
            (false, true) => DragState.Grid2Inv,
            _ => DragState.Grid2Grid,
        };
    }
    private bool IsLayer(int gameObjectLayer, LayerMask layer)
    {
        return ((1 << gameObjectLayer) & layer) != 0;
    }
}