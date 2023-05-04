using System.Collections.Generic;
using System.Linq;
using TMPro;

public class ChampionSelectionState : GameState
{
    GameManager gameManager;
    // Logic for entering Champion Selection state
    public override void OnEnterState()
    {
        duration = 30f;
        GameManager gameManager = GameManager.Instance;
        GridSystemVisual.Instance.ShowAllGridPosition();
        gameManager.gridSizeTextMesh.gameObject.SetActive(true);
        CardShop cardShop = CardShop.Instance;
        cardShop.OpenShopMenu();
        cardShop.RandomSelect5ItemForShopFree();

        List<Unit> allUnits = gameManager.GetAllUnits;

        allUnits.Select(u => u.GetComponent<IDamageable>()).
            ToList().ForEach(d => d.Heal(999999f));
    }
    // Logic for updating Champion Selection state
    public override void OnUpdate()
    {

    }
    // Logic for exiting Champion Selection state
    public override void OnExitState()
    {
        GameManager gameManager = GameManager.Instance;

        // Check if there is free space on the grid and if there are units in the inventory
        bool onGridHaveFreeSpace = gameManager.GetAllUnitsOnGrid.Count < Economy.Level;
        bool unitsInInventory = gameManager.GetAllUnitsOnInventory.Any();

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
                    unit.TeleportToPosition(LevelGrid.Instance.GetWorldPosition(lastGridPosition), gridPosition);
                    InventoryGrid.Instance.RemoveAnyUnitAtInventoryPosition(lastGridPosition);
                    LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, unit);
                    unit.OnGrid = true;
                });
        }

        GridSystemVisual.Instance.HideAllGridPosition();
        gameManager.gridSizeTextMesh.gameObject.SetActive(false);
    }
}
