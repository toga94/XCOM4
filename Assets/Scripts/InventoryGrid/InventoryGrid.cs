using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryGrid : Singleton<InventoryGrid>
{


    public event EventHandler<OnAnyUnitMovedInventoryPositionEventArgs> OnAnyUnitMovedInventoryPosition;

    public event EventHandler<OnAnyUnitSwappedInventoryPositionEventArgs> OnAnyUnitSwappedInventoryPosition;

    public event EventHandler<Unit> OnAnyUnitAddedInventoryPosition;

    public class OnAnyUnitMovedInventoryPositionEventArgs : EventArgs
    {
        public Unit unit;
        public GridPosition fromGridPosition;
        public GridPosition toGridPosition;
    }

    public class OnAnyUnitSwappedInventoryPositionEventArgs : EventArgs
    {
        public Unit unitA;
        public GridPosition gridPositionA;
        public Unit unitB;
        public GridPosition gridPositionB;
    }

    [SerializeField] private int inventoryWidth = 6;
    [SerializeField] private int inventoryHeight = 10;
    [SerializeField] private float inventoryCellSize = 1;

    private GridSystem<GridObject> inventorySystem;


    private void Awake()
    {
        inventorySystem = new GridSystem<GridObject>(inventoryWidth, inventoryHeight, inventoryCellSize,
        (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition), new Vector3Int(-Mathf.FloorToInt(inventoryCellSize * 5), 0, -Mathf.FloorToInt(inventoryCellSize * 5)));
    }

    public void AddUnitAtInventoryPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = inventorySystem.GetGridObject(gridPosition);
        gridObject.AddUnit(unit);
        OnAnyUnitMovedInventoryPosition?.Invoke(this, new OnAnyUnitMovedInventoryPositionEventArgs
        {
            unit = unit,
            fromGridPosition = new GridPosition(0, 0),
            toGridPosition = gridPosition,
        });

        OnAnyUnitAddedInventoryPosition?.Invoke(this, unit);
    }
    public List<Unit> GetUnitListAtInventoryPosition(GridPosition gridPosition)
    {
        GridObject gridObject = inventorySystem.GetGridObject(gridPosition);
        return gridObject.GetUnitList();
    }
    public void RemoveUnitAtInventoryPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = inventorySystem.GetGridObject(gridPosition);
        gridObject.RemoveUnit(unit);

    }
    public void RemoveAnyUnitAtInventoryPosition(GridPosition gridPosition)
    {
        GridObject gridObject = inventorySystem.GetGridObject(gridPosition);
        gridObject.RemoveAnyUnit();
    }
    public void SellAnyUnitAtInventoryPosition(GridPosition gridPosition)
    {
        GridObject gridObject = inventorySystem.GetGridObject(gridPosition);
        gridObject.RemoveAnyUnit();
        Destroy(gridObject.GetUnit().gameObject);
    }
    public void UnitMovedInventoryPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition)
    {
        //  RemoveUnitAtGridPosition(fromGridPosition, unit);
        RemoveUnitAtInventoryPosition(fromGridPosition, unit);
        RemoveAnyUnitAtInventoryPosition(fromGridPosition);
        AddUnitAtInventoryPosition(toGridPosition, unit);

        OnAnyUnitMovedInventoryPosition?.Invoke(this, new OnAnyUnitMovedInventoryPositionEventArgs
        {
            unit = unit,
            fromGridPosition = fromGridPosition,
            toGridPosition = toGridPosition,
        });
    }

    public void UnitSwappedInventoryPosition(Unit unitA, Unit unitB, GridPosition gridPositionA, GridPosition gridPositionB)
    {

        RemoveUnitAtInventoryPosition(gridPositionA, unitA);
        RemoveUnitAtInventoryPosition(gridPositionB, unitB);
        RemoveAnyUnitAtInventoryPosition(gridPositionA);
        RemoveAnyUnitAtInventoryPosition(gridPositionB);

        AddUnitAtInventoryPosition(gridPositionA, unitB);
        AddUnitAtInventoryPosition(gridPositionB, unitA);


        unitA.TeleportToPosition(GetInventoryWorldPosition(gridPositionA), gridPositionA);
        unitB.TeleportToPosition(GetInventoryWorldPosition(gridPositionB), gridPositionB);

        OnAnyUnitSwappedInventoryPosition?.Invoke(this, new OnAnyUnitSwappedInventoryPositionEventArgs
        {
            unitA = unitA,
            gridPositionA = gridPositionA,
            unitB = unitB,
            gridPositionB = gridPositionB,
        });
    }


    public GridPosition GetInventoryPosition(Vector3 worldPosition) => inventorySystem.GetGridPosition(worldPosition);

    public Vector3 GetInventoryWorldPosition(GridPosition gridPosition) => inventorySystem.GetWorldPosition(gridPosition);

    public bool IsValidInventoryPosition(GridPosition gridPosition) => inventorySystem.IsValidGridPosition(gridPosition);

    public int GetWidth() => inventorySystem.GetWidth();

    public int GetHeight() => inventorySystem.GetHeight();

    public bool HasAnyUnitOnInventoryPosition(GridPosition gridPosition)
    {
        GridObject gridObject = inventorySystem.GetGridObject(gridPosition);
        return gridObject.HasAnyUnit();
    }

    public Unit GetUnitAtInventoryPosition(GridPosition gridPosition)
    {
        GridObject gridObject = inventorySystem.GetGridObject(gridPosition);
        return gridObject.GetUnit();
    }


}
