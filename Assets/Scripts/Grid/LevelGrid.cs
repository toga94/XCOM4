using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{

    public static LevelGrid Instance { get; private set; }


    public event EventHandler<OnAnyUnitMovedGridPositionEventArgs> OnAnyUnitMovedGridPosition;

    public event EventHandler<OnAnyUnitSwappedGridPositionEventArgs> OnAnyUnitSwappedGridPosition;


    public class OnAnyUnitMovedGridPositionEventArgs : EventArgs
    {
        public Unit unit;
        public GridPosition fromGridPosition;
        public GridPosition toGridPosition;
    }

    public class OnAnyUnitSwappedGridPositionEventArgs : EventArgs
    {
        public Unit unitA;
        public GridPosition gridPositionA;
        public Unit unitB;
        public GridPosition gridPositionB;
    }




    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private float cellSize = 1;



    private GridSystemHex<GridObjectHex> gridSystem;



    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one LevelGrid! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        gridSystem = new GridSystemHex<GridObjectHex>(width, height, cellSize,
                 (GridSystemHex<GridObjectHex> g, GridPosition gridPosition) => new GridObjectHex(g, gridPosition));



        //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
    }




    /// <summary>
    /// //////////
    /// </summary>
    /// <param name="gridPosition"></param>
    /// <param name="unit"></param>

    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObjectHex gridObject = gridSystem.GetGridObjectHex(gridPosition);
        gridObject.AddUnit(unit);
        OnAnyUnitMovedGridPosition?.Invoke(this, new OnAnyUnitMovedGridPositionEventArgs
        {
            unit = unit,
            fromGridPosition = new GridPosition(0,0),
            toGridPosition = gridPosition,
        });
    }

    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
    {
        GridObjectHex gridObject = gridSystem.GetGridObjectHex(gridPosition);
        return gridObject.GetUnitList();
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        var unitPos = unit.UnitGridPosition;
        if (!IsValidGridPosition(unitPos))
        {
            Debug.LogError("NotIsValidGridPosition");
            return;
        }
        GridObjectHex gridObject = gridSystem.GetGridObjectHex(unitPos);
        if (!gridObject.HasAnyUnit())
        {
           // Debug.LogError(unitPos.ToString() + " is empty");
            return;
        }
        gridObject.RemoveUnit(unit);

    }
    public void RemoveAnyUnitAtGridPosition(GridPosition gridPosition)
    {
        GridObjectHex gridObject = gridSystem.GetGridObjectHex(gridPosition);
        gridObject.RemoveAnyUnit();
    }
    public void SellAnyUnitAtGridPosition(GridPosition gridPosition)
    {
        GridObjectHex gridObject = gridSystem.GetGridObjectHex(gridPosition);
        gridObject.RemoveAnyUnit();
        Destroy(gridObject.GetUnit().gameObject);
    }
    public void UnitMovedGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition)
    {
        //  RemoveUnitAtGridPosition(fromGridPosition, unit);
        RemoveUnitAtGridPosition(fromGridPosition, unit);
        RemoveAnyUnitAtGridPosition(fromGridPosition);
        AddUnitAtGridPosition(toGridPosition, unit);

        OnAnyUnitMovedGridPosition?.Invoke(this, new OnAnyUnitMovedGridPositionEventArgs
        {
            unit = unit,
            fromGridPosition = fromGridPosition,
            toGridPosition = toGridPosition,
        });
    }
    public void UnitSwappedGridPosition(Unit unitA, Unit unitB, GridPosition gridPositionA, GridPosition gridPositionB)
    {

        RemoveUnitAtGridPosition(gridPositionA, unitA);
        RemoveUnitAtGridPosition(gridPositionB, unitB);
        RemoveAnyUnitAtGridPosition(gridPositionA);
        RemoveAnyUnitAtGridPosition(gridPositionB);

        AddUnitAtGridPosition(gridPositionA, unitB);
        AddUnitAtGridPosition(gridPositionB, unitA);


       unitA.TeleportToPosition(GetWorldPosition(gridPositionA), gridPositionA);
       unitB.TeleportToPosition(GetWorldPosition(gridPositionB), gridPositionB);

        OnAnyUnitSwappedGridPosition?.Invoke(this, new OnAnyUnitSwappedGridPositionEventArgs
        {
            unitA = unitA,
            gridPositionA = gridPositionA,
            unitB = unitB,
            gridPositionB = gridPositionB,
        });
    }


    public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);

    public Vector3 GetWorldPosition(GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);

    public bool IsValidGridPosition(GridPosition gridPosition) => gridSystem.IsValidGridPosition(gridPosition);

    public int GetWidth() => gridSystem.GetWidth();

    public int GetHeight() => gridSystem.GetHeight();

    public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
    {
        GridObjectHex gridObject = gridSystem.GetGridObjectHex(gridPosition);
        return gridObject.HasAnyUnit();
    }

    public Unit GetUnitAtGridPosition(GridPosition gridPosition)
    {
        GridObjectHex gridObject = gridSystem.GetGridObjectHex(gridPosition);
        return gridObject.GetUnit();
    }
}
