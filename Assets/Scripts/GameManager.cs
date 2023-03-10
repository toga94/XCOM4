using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int GetPlayerLevel { get; }
    public int GetPlayerCoin { get; }

    public List<Unit> UnitsInGrid;
    public List<Unit> UnitsInInventory;

    public event EventHandler<UpdateTextArg> OnUpdateText;


    public UnitObject[] unitObjects;


    public class UpdateTextArg : EventArgs {

    }

    private List<Unit> alllUnits;


    public List<Unit> GetAllUnits
    {
        get
        {
            List<Unit> units = new List<Unit>();
            Unit[] allUnits = FindObjectsOfType<Unit>();
            foreach (Unit unit in allUnits)
            {
                units.Add(unit);
            }
            return units;
        }
    }

    public List<Unit> GetUnitsByNameAndLevel(string name)
    {
        List<Unit> units = new List<Unit>();
        Unit[] allUnits = FindObjectsOfType<Unit>();
        foreach (Unit unit in allUnits)
        {
            if (unit.GetUnitName == name)
            {
                units.Add(unit);
            }
            if (units.Count == 3)
            {
                break;
            }
        }
        return units;
    }

    public List<Unit> GetAllUnitsOnInventory
    {
        get
        {
            List<Unit> units = new List<Unit>();
            Unit[] allUnits = FindObjectsOfType<Unit>();
            foreach (Unit unit in allUnits)
            {
                if (!unit.OnGrid) units.Add(unit);
            }
            return units;
        }
    }
    public List<Unit> GetAllUnitsOnGrid
    {
        get
        {
            List<Unit> units = new List<Unit>();
            Unit[] allUnits = FindObjectsOfType<Unit>();
            foreach (Unit unit in allUnits)
            {
                if (unit.OnGrid) units.Add(unit);
            }
            return units;
        }
    }

    private void CalculateUnits(object sender, EventArgs e)
    {
        alllUnits = GetAllUnits;



    }

    void UpdateMeText() {
        OnUpdateText?.Invoke(this, new UpdateTextArg { });
    }


    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one GameManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (UnitsInGrid.Count > 0)
            AddUnitsToGrid();
        if (UnitsInInventory.Count > 0)
            AddUnitsToInventory();


        LevelGrid.Instance.OnAnyUnitMovedGridPosition += CalculateUnits;

        UpdateMeText();
        // SpawnUnitAtInventory("Lina");  Spawn unit with name
    }
    private void AddUnitsToGrid()
    {
        int unitIndex = 0;
        LevelGrid levelGrid = LevelGrid.Instance;
        int width = levelGrid.GetWidth() - 1;
        int height = levelGrid.GetHeight() - 1;

        foreach (var unit in UnitsInGrid)
        {
            if (unitIndex >= width * height)
            {
                break;
            }

            int index = unitIndex++;
            int x = index % width;
            int z = index / width;
            unit.OnGrid = true;
            GridPosition gridPosition = new GridPosition(x, z);
            levelGrid.AddUnitAtGridPosition(gridPosition, unit);
            unit.Move(levelGrid.GetWorldPosition(gridPosition));
        }
    }
    private void AddUnitsToInventory()
    {
        int unitIndex = 0;
        InventoryGrid inventoryGrid = InventoryGrid.Instance;
        int width = inventoryGrid.GetWidth() - 1;
      

        foreach (var unit in UnitsInInventory)
        {
            if (unitIndex >= width)
            {
                break;
            }
            unit.OnGrid = false;
            int x = unitIndex++;
            int z = 0;

            GridPosition gridPosition = new GridPosition(x, z);
            inventoryGrid.AddUnitAtInventoryPosition(gridPosition, unit);
            unit.Move(inventoryGrid.GetInventoryWorldPosition(gridPosition));
        }
    }
    public void SpawnUnitAtInventory(string unitName)
    {
        InventoryGrid inventoryGrid = InventoryGrid.Instance;
        int width = inventoryGrid.GetWidth() - 1;
        if (GetAllUnitsOnInventory.Count >= width)
        {
            Debug.LogError("Inventory is full!");
            return;
        }
        foreach (var item in unitObjects)
        {
            if (item.unitName == unitName) {
                Unit SpawnedUnit = GameObject.Instantiate(item.Prefab, Vector3.zero, Quaternion.identity).GetComponent<Unit>();
                AddUnitToInventory(SpawnedUnit);
            }
        }


    }
    private void AddUnitToInventory(Unit unit)
    {
        InventoryGrid inventoryGrid = InventoryGrid.Instance;
        int width = inventoryGrid.GetWidth() - 1;

        if (GetAllUnitsOnInventory.Count > width)
        {
            Debug.LogError("Inventory is full!");
            return;
        }

        unit.OnGrid = false;
        int x = UnitsInInventory.Count;

        GridPosition gridPosition = GetNextFreeGridPosition();

        Debug.Log("gridpos: " + gridPosition);
        inventoryGrid.AddUnitAtInventoryPosition(gridPosition, unit);
        unit.Move(inventoryGrid.GetInventoryWorldPosition(gridPosition));
        UnitsInInventory.Add(unit);

        CheckForUpgrade(unit);
    }
    //Debug.Log(unit.GetUnitName + "Spawned count is " + GetUnitsByNameAndLevel(unit.GetUnitName).Count);

    private void CheckForUpgrade(Unit unit)
    {
        var upgredableUnit = GetUnitsByNameAndLevel(unit.GetUnitName);
        if (upgredableUnit.Count > 2)
        {
  
            upgredableUnit = upgredableUnit.OrderBy(u => !u.OnGrid).ToList();

            List<Unit> nonGridUnits = upgredableUnit.Where(u => !u.OnGrid).ToList();
            if (nonGridUnits.Count >= 2)
            {
                Destroy(nonGridUnits[0].gameObject);
                Destroy(nonGridUnits[1].gameObject);

                Unit highestLevelUnit = upgredableUnit.OrderByDescending(u => u.GetUnitLevel).FirstOrDefault();
                if (highestLevelUnit != null)
                {
                    highestLevelUnit.UpgradeLevel();
                }
            }

        }
    }

    private GridPosition GetNextFreeGridPosition()
    {
        InventoryGrid inventoryGrid = InventoryGrid.Instance;
        for (int x = 0; x < UnitsInInventory.Count; x++)
        {
            GridPosition gridPosition = new GridPosition(x, 0);
            if (!inventoryGrid.HasAnyUnitOnInventoryPosition(gridPosition))
            {
                return gridPosition;
            }
        }

        return new GridPosition(UnitsInInventory.Count, 0);
    }
}
