using TMPro;

public class ChampionSelectionState : GameState
{
    TextMeshPro gridSizeTextMesh;
    // Logic for entering Champion Selection state
    public override void OnEnterState()
    {
        GridSystemVisual.Instance.ShowAllGridPosition();
        GameManager.Instance.gridSizeTextMesh.gameObject.SetActive(true);
        CardShop cardShop = CardShop.Instance;
        cardShop.OpenShopMenu();
        cardShop.RandomSelect5ItemForShopFree();
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
