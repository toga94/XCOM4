using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChampionSelectionState : GameState
{
    private GameManager gameManager;
    private List<TransformData> unitTransforms = new List<TransformData>();
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

        LoadAndSaveUnitsPosition();

        List<Unit> allUnits = gameManager.GetAllUnits;

        allUnits.Select(u => u.GetComponent<HealthSystem>()).
            ToList().ForEach(d =>
            {
                d.Heal(999999f);
                d.DecreaseMana(d.GetMana);
            });
    }

    private void LoadAndSaveUnitsPosition()
    {
        unitTransforms = gameManager.GetAllUnitsOnGrid.Select(unit =>
        new TransformData(
            new Vector3(unit.UnitGridPosition.x, unit.transform.position.y, unit.UnitGridPosition.z),
        unit.transform.rotation)).ToList();
        gameManager.SavedUnitTransforms = unitTransforms;
        gameManager = GameManager.Instance;

        List<Unit> units = gameManager.GetAllUnitsOnGrid;
        List<TransformData> savedTransforms = gameManager.SavedUnitTransforms;
        units.Select((unit, index) => new { unit, index }).ToList().ForEach(obj =>
        {
            obj.unit.transform.SetPositionAndRotation(
                savedTransforms[obj.index].position, savedTransforms[obj.index].rotation);
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
        LoadAndSaveUnitsPosition();
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
