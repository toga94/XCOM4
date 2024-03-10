using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using MoreMountains.Tools;
public class ChampionSelectionState : GameState
{
    private GameManager gameManager;
    private List<TransformData> unitTransforms = new List<TransformData>();
    private List<Unit> unitsOnGrid;
    private Transform gridSizeMonitor;
    // Logic for entering Champion Selection state
    public override void OnEnterState()
    {

        duration = 15f;
        gameManager = GameManager.Instance;
        GridSystemVisual.Instance.ShowAllGridPosition();
        gridSizeMonitor = gameManager.gridSizeTextMesh.transform;
        gridSizeMonitor.DOMove(
            new Vector3(gridSizeMonitor.position.x, 1.67f, gridSizeMonitor.position.z), 1f
            ).SetEase(Ease.OutElastic);
        // gameManager.gridSizeTextMesh.gameObject.SetActive(true);
        CardShop cardShop = CardShop.Instance;
        cardShop.OpenShopMenu();
        cardShop.RandomSelect5ItemForShopFree();
        UnitPositionUtility.RefreshUnitsPosition();
        

        List<Unit> allUnits = gameManager.GetAllUnits;

        allUnits.Select(u => u.GetComponent<HealthSystem>()).
            ToList().ForEach(d =>
            {
                d.Heal();
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
        List<Unit> onGridUnit = gameManager.GetAllUnitsOnGrid;
        List<Unit> onInvertoryUnit = gameManager.GetAllUnitsOnInventory;
        // Check if there is free space on the grid and if there are units in the inventory
        bool onGridHaveFreeSpace = onGridUnit.Count < EconomyManager.Level;
        bool unitsInInventory = onInvertoryUnit.Count > 0;

        if (onGridHaveFreeSpace && unitsInInventory)
        {
            // Move units from inventory to grid
            onInvertoryUnit
                .Take(EconomyManager.Level - onGridUnit.Count)
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

        gridSizeMonitor.DOMove(
    new Vector3(gridSizeMonitor.position.x, 35f, gridSizeMonitor.position.z), 2f
    ).SetEase(Ease.InFlash);
      
    }


}
