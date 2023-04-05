﻿using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] private GameObject sellUI;
    [SerializeField] private GameObject marketUI;

    [SerializeField] private DragState dragState = DragState.Inv2Inv;
    private bool selling;

    public void SetSelling(bool value) => selling = value;


    private GridPosition lastgridPosition;
    private GridPosition gridPosition;

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
        TouchPhase touchPhase = touch.phase;

        if (touchPhase == TouchPhase.Began)
        {
            StartDragging(touch);
        }

        else if (_draggableObject && touchPhase == TouchPhase.Moved)
        {
            character.GetUnit.charState = CharState.Fall;
            MoveDraggableObject(touch);

        }

        else if (_draggableObject && (touchPhase == TouchPhase.Ended || touchPhase == TouchPhase.Canceled))
        {
            if (selling)
            {
                SellUnit();
                ResetUI();
            }
            else if (!selling)
            {
                character.GetUnit.charState = CharState.Idle;
                StopDragging();
                ResetUI();
            }

        }
    }

    private void SellUnit()
    {
        GameManager gameManager = GameManager.Instance;
        Unit unit = character.GetUnit;
        RareOptions rareOptions = unit.GetUnitObject.rareOptions ;

        int unitCost = Economy.GetUnitCost(unit.GetUnitLevel, rareOptions);
        GameManager.Instance.AddGold(unitCost );

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

    private void StartDragging(Touch touch)
    {
        Ray ray = _mainCamera.ScreenPointToRay(touch.position);
        if (Physics.Raycast(ray, out RaycastHit hit) && IsLayer(hit.collider.gameObject.layer, draggableLayer))
        {
            sellUI.SetActive(true);
            marketUI.SetActive(false);
            _draggableObject = hit.transform;

            character = new Character(
                _draggableObject,
                hit.transform.GetComponent<Collider>(),
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
            float y = Mathf.Floor(hit.point.y / gridSize) * gridSize + 1.5f;
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
            if (!lastfloor)
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
                    if (!levelGrid.HasAnyUnitOnGridPosition(gridPosition))
                    {
                        Unit unit = character.GetUnit;

                        unit.TeleportToPosition(levelGrid.GetWorldPosition(gridPosition), gridPosition);
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