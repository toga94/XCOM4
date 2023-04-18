using System.Collections.Generic;
using TMPro;

public class ChampionSelectionState : GameState
{
    TextMeshPro gridSizeTextMesh;
    // Logic for entering Champion Selection state
    public override void OnEnterState()
    {
        GameManager gameManager = GameManager.Instance;
        GridSystemVisual.Instance.ShowAllGridPosition();
        gameManager.gridSizeTextMesh.gameObject.SetActive(true);
        CardShop cardShop = CardShop.Instance;
        cardShop.OpenShopMenu();
        cardShop.RandomSelect5ItemForShopFree();

        List<Unit> allUnits = gameManager.GetAllUnits;

        foreach (var unit in allUnits)
        {
            unit.GetComponent<IDamageable>().Heal(999999f);
        }
    }
    // Logic for updating Champion Selection state
    public override void OnUpdate()
    {

    }
    // Logic for exiting Champion Selection state
    public override void OnExitState()
    {
        GridSystemVisual.Instance.HideAllGridPosition();
        GameManager.Instance.gridSizeTextMesh.gameObject.SetActive(false);
    }
}
