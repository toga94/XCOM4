using System;
using System.Collections;
using System.Collections.Generic;
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
        if(UnitsInGrid.Count > 0)
            AddUnitsToGrid();
        if (UnitsInInventory.Count > 0)
            AddUnitsToInventory();

        UpdateMeText();
        SpawnUnitAtInventory("Lina");
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
        if (UnitsInInventory.Count >= width)
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

        if (UnitsInInventory.Count >= width)
        {
            Debug.LogError("Inventory is full!");
            return;
        }

        unit.OnGrid = false;
        int x = UnitsInInventory.Count;
        int z = 0;

        GridPosition gridPosition = new GridPosition(x, z);
        inventoryGrid.AddUnitAtInventoryPosition(gridPosition, unit);
        unit.Move(inventoryGrid.GetInventoryWorldPosition(gridPosition));

        UnitsInInventory.Add(unit);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
           
        }
    }
}
