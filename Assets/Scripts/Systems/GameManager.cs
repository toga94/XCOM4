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

    public bool allMinionsisDead;
    public class UpdateTextArg : EventArgs
    {

    }

    private List<Unit> alllUnits;


    public List<Unit> GetAllUnits
    {
        get
        {
            return FindObjectsOfType<Unit>().Where(u => u.isOwn)
                   .ToList();
        }
    }
    public List<Unit> GetAllEnemyUnits
    {
        get
        {
            return FindObjectsOfType<Unit>().Where(u => !u.isOwn)
                   .ToList();
        }
    }
    public List<Unit> GetUnitsByNameAndLevel(string name)
    {
        return FindObjectsOfType<Unit>()
               .Where(u => u.GetUnitNameWithLevel == name && u.isOwn)
               .Take(3)
               .ToList();
    }
    public List<Unit> GetEnemyUnitsByNameAndLevel(string name)
    {
        return FindObjectsOfType<Unit>()
               .Where(u => u.GetUnitNameWithLevel == name && u.isOwn)
               .Take(3)
               .ToList();
    }
    public List<Unit> GetAllUnitsOnInventory
    {
        get
        {
            return FindObjectsOfType<Unit>()
                   .Where(u => !u.OnGrid && u.isOwn)
                   .ToList();
        }
    }

    public List<Unit> GetAllEnemyUnitsOnInventory
    {
        get
        {
            return FindObjectsOfType<Unit>()
                   .Where(u => !u.OnGrid && u.isOwn)
                   .ToList();
        }
    }

    public List<Unit> GetAllUnitsOnGrid
    {
        get
        {
            return FindObjectsOfType<Unit>()
                   .Where(u => u.OnGrid && u.isOwn)
                   .ToList();
        }
    }
    public List<Unit> GetAllEnemyUnitsOnGrid
    {
        get
        {
            return FindObjectsOfType<Unit>()
                   .Where(u => u.OnGrid && u.isOwn)
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
        int unitsOnGrid = GetAllUnitsOnGrid.Count;
        gridSizeTextMesh.text = $"{unitsOnGrid}/{curLevel}";
        Color32 labelColor = unitsOnGrid < curLevel ? 
            new Color(1, 1, 1, 1) : new Color(0.5f, 0.5f, 0.5f, 1);
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

    GameState currentGameState;
    private void OnStateChanged(GameState gameState)
    {
        currentGameState = gameState;
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

    public void SpawnUnitAtInventory(string unitName, bool isOwn)
    {
        if (InventoryIsFull()) return;
        foreach (UnitObject item in unitObjects)
        {
            if (item.unitName == unitName)
            {
                Unit SpawnedUnit = Instantiate(item.Prefab, Vector3.zero, Quaternion.identity).GetComponent<Unit>();
                SpawnedUnit.gameObject.name = SpawnedUnit.GetUnitNameWithLevel;
                SpawnedUnit.isOwn = isOwn;
                AddUnitToInventory(SpawnedUnit);
            }
        }
    }
    private void AddUnitToInventory(Unit unit)
    {
        if (InventoryIsFull())
        {
            return;
        }

        unit.OnGrid = false;

        GridPosition gridPosition = GetNextFreeInventoryGridPosition();

        inventoryGrid.AddUnitAtInventoryPosition(gridPosition, unit);
        unit.TeleportToPosition(inventoryGrid.GetInventoryWorldPosition(gridPosition), gridPosition);
        UnitsInInventory.Add(unit);

        CheckForUpgradeForAll();
    }

    private void CheckForUpgradeForAll()
    {
        GetAllUnits.ForEach(selectedUnit => CheckForUpgrade(selectedUnit));
    }

    private void CheckForUpgrade(Unit unit)
    {

        IEnumerable<Unit> upgradableUnits = GetUnitsByNameAndLevel(unit.GetUnitNameWithLevel);
        if (GameStateSystem.Instance.CurrentState.IsCombatState)
        {
            upgradableUnits = upgradableUnits.Where(u => !u.OnGrid && u.isOwn);
        }

        if (upgradableUnits.Count() >= 3)
        {
            IOrderedEnumerable<Unit> highestLevelUnits = upgradableUnits
                .OrderByDescending(u => u.OnGrid)
                .ThenBy(u => u.UnitGridPosition.x)
                .ThenBy(u => u.UnitGridPosition.z);

            Unit highestLevelUnit = highestLevelUnits.FirstOrDefault();

            if (highestLevelUnit != null)
            {
                Instantiate(levelUpFx, highestLevelUnit.transform.position + Vector3.up / 2, Quaternion.identity);
                highestLevelUnit.UpgradeLevel();
                upgradableUnits = upgradableUnits.Except(new[] { highestLevelUnit });

                if (upgradableUnits.Count() >= 2)
                {
                    RemoveOtherUnitsFromList(upgradableUnits.ToList());
                }
            }
        }
    }
    public bool OnlyCheckForUpgrade(Unit unit)
    {
        return GetUnitsByNameAndLevel(unit.GetUnitNameWithLevel).Count >= 2;
    }
    public bool CanIUpgradeTo2Star(Unit unit)
    {
        return GetUnitsByNameAndLevel($"{unit.GetUnitName}{0}").Count >= 2;
    }
    public bool CanIUpgradeTo2StarFrom3(Unit unit)
    {
        return GetUnitsByNameAndLevel($"{unit.GetUnitName}{0}").Count >= 2;
    }
    public bool CanIUpgradeTo3Star(Unit unit)
    {
        return GetUnitsByNameAndLevel($"{unit.GetUnitName}{1}").Count >= 2 && CanIUpgradeTo2StarFrom3(unit);
    }
    private void RemoveOtherUnitsFromList(List<Unit> nonGridUnits)
    {
        LevelGrid levelGrid = LevelGrid.Instance;
        InventoryGrid inventoryGrid = InventoryGrid.Instance;
        foreach (Unit unit in nonGridUnits)
        {

            if (unit.OnGrid)
            {
                levelGrid.RemoveAnyUnitAtGridPosition(unit.UnitGridPosition);
            }
            else
            {
                inventoryGrid.RemoveAnyUnitAtInventoryPosition(unit.UnitGridPosition);
            }

            Destroy(unit.gameObject);
        }
    }

    public GridPosition GetNextFreeInventoryGridPosition()
    {
        InventoryGrid inventoryGrid = InventoryGrid.Instance;
        int maxIndex = inventoryGrid.GetWidth() - 1;

        IEnumerable<GridPosition> freePositions = Enumerable.Range(0, maxIndex)
            .Select(x => new GridPosition(x, 0))
            .Where(gp => !inventoryGrid.HasAnyUnitOnInventoryPosition(gp));

        return freePositions.DefaultIfEmpty(new GridPosition(maxIndex, 0)).First();
    }
    public GridPosition GetNextFreeLevelGridPosition()
    {
        LevelGrid grid = LevelGrid.Instance;
        int maxIndex = grid.GetWidth() - 1;

        IEnumerable<GridPosition> freePositions = Enumerable.Range(0, maxIndex)
            .Select(x => new GridPosition(x, 0))
            .Where(gp => !grid.HasAnyUnitOnGridPosition(gp));

        return freePositions.DefaultIfEmpty(new GridPosition(maxIndex, 0)).First();
    }
    private int winStreak = 0;

    public void WinCombat()
    {
        // Increment the player's win streak when they win a combat
        winStreak++;
    }

    public void LoseCombat()
    {
        // Reset the player's win streak when they lose a combat
        winStreak = 0;
    }

    public int GetWinStreak()
    {
        return winStreak;
    }

}
