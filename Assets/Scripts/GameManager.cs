using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int PlayerLevel { get => playerLevel;  }
    public int PlayerCoin { get => playerCoin; }

    private int playerLevel;
    private int playerCoin;
    public List<Unit> UnitsInGrid;
    public List<Unit> UnitsInInventory;





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
            AddToGrid();
        if (UnitsInInventory.Count > 0)
            AddToInventory();

    }
    private void AddToGrid()
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

            GridPosition gridPosition = new GridPosition(x, z);
            LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, unit);
            unit.Move(LevelGrid.Instance.GetWorldPosition(gridPosition));
        }
    }
    private void AddToInventory()
    {
        int unitIndex = 0;
        InventoryGrid inventoryGrid = InventoryGrid.Instance;
        int width = inventoryGrid.GetWidth() - 1;
        int height = inventoryGrid.GetHeight() - 1;

        foreach (var unit in UnitsInInventory)
        {
            if (unitIndex >= width * height)
            {
                break;
            }

            int index = unitIndex++;
            int x = index % width;
            int z = index / width;

            GridPosition gridPosition = new GridPosition(x, z);
            inventoryGrid.AddUnitAtInventoryPosition(gridPosition, unit);
            unit.Move(inventoryGrid.GetInventoryWorldPosition(gridPosition));
        }
    }
    void Update()
    {
        
    }
}
