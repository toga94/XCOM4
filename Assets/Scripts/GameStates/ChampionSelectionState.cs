using System.Collections.Generic;
using System.Linq;
using TMPro;

public class ChampionSelectionState : GameState
{
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
        GridSystemVisual.Instance.HideAllGridPosition();
        GameManager.Instance.gridSizeTextMesh.gameObject.SetActive(false);
    }
}
