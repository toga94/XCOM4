using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class GameManager : Singleton<GameManager>
{
    public int GetPlayerCoin { get; }

    public List<Unit> UnitsInGrid;
    public List<Unit> UnitsInInventory;


    public TextMeshPro gridSizeTextMesh;
    private SpriteRenderer gridSizeIcon;
    private InventoryGrid inventoryGrid;
    private LevelGrid levelgrid; 
    private UnityEngine.Object levelUpFx;
    public event EventHandler<UpdateTextArg> OnUpdateText;

    public UnitObject[] unitObjects;

    [SerializeField] private int gold;
    public int GetGold => gold;


    public class UpdateTextArg : EventArgs
    {

    }

    private List<Unit> alllUnits;


    public List<Unit> GetAllUnits
    {
        get
        {
            return FindObjectsOfType<Unit>()
                   .ToList();
        }
    }

    public List<Unit> GetUnitsByNameAndLevel(string name)
    {
        return FindObjectsOfType<Unit>()
               .Where(u => u.GetUnitNameWithLevel == name)
               .Take(3)
               .ToList();
    }
    public List<Unit> GetAllUnitsOnInventory
    {
        get
        {
            return FindObjectsOfType<Unit>()
                   .Where(u => !u.OnGrid)
                   .ToList();
        }
    }
    public List<Unit> GetAllUnitsOnGrid
    {
        get
        {
            return FindObjectsOfType<Unit>()
                   .Where(u => u.OnGrid)
                   .ToList();
        }
    }

    public List<TransformData> SavedUnitTransforms { get;  set; }

    private void CalculateUnits(object sender, EventArgs e)
    {
        alllUnits = GetAllUnits;
        Invoke(nameof (UpdateGridSizeTextAndIcon), 0.15f);
    }
    private void CalculateUnits(int value)
    {
        alllUnits = GetAllUnits;
        UpdateGridSizeTextAndIcon();
    }

    private void UpdateGridSizeTextAndIcon()
    {
        int curLevel = Economy.Level;
        gridSizeTextMesh.text = $"{GetAllUnitsOnGrid.Count}/{curLevel}";
        Color32 labelColor = GetAllUnitsOnGrid.Count < curLevel ? new Color(1, 1, 1, 1) : new Color(0.5f, 0.5f, 0.5f, 1);
        gridSizeTextMesh.faceColor = labelColor;
        gridSizeIcon.color = labelColor;
    }

    private void UpdateMeText() => OnUpdateText?.Invoke(this, new UpdateTextArg { });

    private void Start()
    {
        LevelGrid levelGridInstance = LevelGrid.Instance;
        levelGridInstance.OnAnyUnitMovedGridPosition += CalculateUnits;
        levelGridInstance.OnAnyUnitSwappedGridPosition += CalculateUnits;

        InventoryGrid InventoryGridInstance = InventoryGrid.Instance;
        InventoryGridInstance.OnAnyUnitMovedInventoryPosition += CalculateUnits;
        InventoryGridInstance.OnAnyUnitSwappedInventoryPosition += CalculateUnits;

        GameStateSystem.Instance.OnGameStateChanged += OnStateChanged;
        Economy.OnExperienceChanged += CalculateUnits;

        if (UnitsInGrid.Count > 0)
            AddUnitsToGrid();
        if (UnitsInInventory.Count > 0)
            AddUnitsToInventory();

        levelUpFx = Resources.Load("FX_LevelUp_01");
        gridSizeTextMesh = transform.GetComponentInChildren<TextMeshPro>();
        gridSizeIcon = gridSizeTextMesh.transform.GetComponentInChildren<SpriteRenderer>();
        UpdateMeText();
        gridSizeTextMesh.text = $"{GetAllUnitsOnGrid.Count}/{Economy.Level}";

        if(Application.platform == RuntimePlatform.Android)
        {
            Application.targetFrameRate = 60;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

            foreach (var unit in GetAllUnits)
            {
                unit.GetComponent<IDamageable>().TakeDamage(10);
            }
        }
    }

    private void OnStateChanged(GameState gameState)
    {
        if (gameState is ChampionSelectionState) {
            CheckForUpgradeForAll();
        }
    }

    private void AddUnitsToGrid()
    {
        int unitIndex = 0;
        LevelGrid levelGrid = LevelGrid.Instance;
        int width = levelGrid.GetWidth() - 1;
        int height = levelGrid.GetHeight() - 1;

        foreach (Unit unit in UnitsInGrid)
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
            unit.TeleportToPosition(levelGrid.GetWorldPosition(gridPosition), gridPosition);
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
            unit.TeleportToPosition(inventoryGrid.GetInventoryWorldPosition(gridPosition), gridPosition);
        }
    }

    public bool InventoryIsFull()
    {
        inventoryGrid = InventoryGrid.Instance;
        int width = inventoryGrid.GetWidth() + 1;
        bool isFull = GetAllUnitsOnInventory.Count >= width;

        if (isFull) Debug.LogError("Inventory is full!");
        return isFull;
    }

    public bool GridisFree()
    {
        return GetAllUnitsOnGrid.Count <= Economy.Level;
    }

    public bool GridIsFull()
    {
        levelgrid = LevelGrid.Instance;
        int width = levelgrid.GetWidth() - 1;
        int height = levelgrid.GetHeight() - 1;
        bool isFull = GetAllUnitsOnGrid.Count + 1 > width * height;

        //if (isFull) Debug.LogError("Grid is full!");
        return isFull;
    }

    public void SpawnUnitAtInventory(string unitName)
    {
        if (InventoryIsFull()) return;
        foreach (var item in unitObjects)
        {
            if (item.unitName == unitName)
            {
                Unit SpawnedUnit = Instantiate(item.Prefab, Vector3.zero, Quaternion.identity).GetComponent<Unit>();
                SpawnedUnit.gameObject.name = SpawnedUnit.GetUnitNameWithLevel;
                AddUnitToInventory(SpawnedUnit);
            }
        }
    }
    private void AddUnitToInventory(Unit unit)
    {
        if (InventoryIsFull()) return;

        unit.OnGrid = false;
        int x = UnitsInInventory.Count;

        GridPosition gridPosition = GetNextFreeGridPosition();

        inventoryGrid.AddUnitAtInventoryPosition(gridPosition, unit);
        unit.TeleportToPosition(inventoryGrid.GetInventoryWorldPosition(gridPosition), gridPosition);
        UnitsInInventory.Add(unit);

        CheckForUpgradeForAll();
    }

    private void CheckForUpgradeForAll()
    {
        foreach (var selectedUnit in GetAllUnits)
        {
            CheckForUpgrade(selectedUnit);
        }
    }

    private void CheckForUpgrade(Unit unit)
    {
        List<Unit> upgradableUnits = GetUnitsByNameAndLevel(unit.GetUnitNameWithLevel);
        if (GameStateSystem.Instance.GetStateIndex != 0)
        {
            upgradableUnits.RemoveAll(u => u.OnGrid);
        }


        if (upgradableUnits.Count >= 3)
        {
            IOrderedEnumerable<Unit> highestLevelUnits = upgradableUnits.OrderByDescending(u => u.OnGrid).ThenBy(u => u.UnitGridPosition.x).ThenBy(u => u.UnitGridPosition.z);


            Unit highestLevelUnit = highestLevelUnits.First();

            if (highestLevelUnit != null)
            {

                Instantiate(levelUpFx, highestLevelUnit.transform.position + Vector3.up / 2, Quaternion.identity);
                highestLevelUnit.UpgradeLevel();
                upgradableUnits.Remove(highestLevelUnit);

                if (upgradableUnits.Count >= 2)
                {
                    RemoveOtherUnitsFromList(upgradableUnits);
                }
            }
        }
    }
    public bool OnlyCheckForUpgrade(Unit unit)
    {
        List<Unit> upgradableUnits = GetUnitsByNameAndLevel(unit.GetUnitNameWithLevel);
        return upgradableUnits.Count >= 2;
    }
    public bool CanIUpgradeTo2Star(Unit unit)
    {
        List<Unit> upgradableUnits = GetUnitsByNameAndLevel($"{unit.GetUnitName}{0}");
        return upgradableUnits.Count >= 2;
    }
    public bool CanIUpgradeTo2StarFrom3(Unit unit)
    {
        List<Unit> upgradableUnits = GetUnitsByNameAndLevel($"{unit.GetUnitName}{0}");
        return upgradableUnits.Count >= 2;
    }
    public bool CanIUpgradeTo3Star(Unit unit)
    {
        List<Unit> upgradableUnits = GetUnitsByNameAndLevel($"{unit.GetUnitName}{1}");
        return upgradableUnits.Count >= 2 && CanIUpgradeTo2StarFrom3(unit);
    }
    private void RemoveOtherUnitsFromList(List<Unit> nonGridUnits)
    {

        foreach (Unit unit in nonGridUnits)
        {

            if (unit.OnGrid)
            {
                LevelGrid.Instance.RemoveAnyUnitAtGridPosition(unit.UnitGridPosition);
            }
            else
            {
                InventoryGrid.Instance.RemoveAnyUnitAtInventoryPosition(unit.UnitGridPosition);
            }

            Destroy(unit.gameObject);
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
