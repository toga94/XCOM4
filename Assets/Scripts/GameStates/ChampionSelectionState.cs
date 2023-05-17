using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChampionSelectionState : GameState
{
    private GameManager gameManager;
    private List<TransformData> unitTransforms = new List<TransformData>();
    private List<Unit> unitsOnGrid;
    // Logic for entering Champion Selection state
    public override void OnEnterState()
    {

        duration = 15f;
        gameManager = GameManager.Instance;
        GridSystemVisual.Instance.ShowAllGridPosition();
        gameManager.gridSizeTextMesh.gameObject.SetActive(true);
        CardShop cardShop = CardShop.Instance;
        cardShop.OpenShopMenu();
        cardShop.RandomSelect5ItemForShopFree();
        UnitPositionUtility.RefreshUnitsPosition();


        List<Unit> allUnits = gameManager.GetAllUnits;

        allUnits.Select(u => u.GetComponent<HealthSystem>()).
            ToList().ForEach(d =>
            {
                d.Heal(999999f);
                d.DecreaseMana(d.GetMana);
            });
    }


    // Logic for updating Champion Selection state
    public override void OnUpdate()
    {

    }
    // Logic for exiting Champion Selection state
    public override void OnExitState()
    {
        gameManager = GameManager.Instance;

        // Check if there is free space on the grid and if there are units in the inventory
        bool onGridHaveFreeSpace = gameManager.GetAllUnitsOnGrid.Count < Economy.Level;
        bool unitsInInventory = gameManager.GetAllUnitsOnInventory.Count > 0;

        if (onGridHaveFreeSpace && unitsInInventory)
        {
            // Move units from inventory to grid
            gameManager.GetAllUnitsOnInventory
                .Take(Economy.Level - gameManager.GetAllUnitsOnGrid.Count)
                .ToList()
                .ForEach(unit =>
                {
                    GridPosition gridPosition = gameManager.GetNextFreeLevelGridPosition();
                    GridPosition lastGridPosition = unit.UnitGridPosition;
                    LevelGrid levelGrid = LevelGrid.Instance;
                    InventoryGrid.Instance.RemoveAnyUnitAtInventoryPosition(lastGridPosition);
                    levelGrid.AddUnitAtGridPosition(gridPosition, unit);
                    unit.OnGrid = true;
                    unit.TeleportToPosition(levelGrid.GetWorldPosition(gridPosition), gridPosition);
                });
        }

        UnitPositionUtility.RefreshUnitsPosition();


        GridSystemVisual.Instance.HideAllGridPosition();
        gameManager.gridSizeTextMesh.gameObject.SetActive(false);
    }
}
