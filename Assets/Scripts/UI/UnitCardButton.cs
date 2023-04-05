using UnityEngine;
using UnityEngine.UI;

public class UnitCardButton : MonoBehaviour
{
    private GameManager gameManager;
    public string CharacterName = "Lina";
    public Image CharacterImage;
    [SerializeField] private Text CharacterLabelText;
    public GameObject TreeStarPanel;
    public GameObject TwoStarPanel;
    public RareOptions rareOptions;
    public Unit unit;
    private void OnEnable()
    {
        gameManager = GameManager.Instance;

    }


    private void LateUpdate()
    {
        bool upgradeTo3Star = GameManager.Instance.GetCountUpgradeTo3Star(unit);
        bool upgradeTo2Star = GameManager.Instance.GetCountUpgradeTo2Star(unit);

        TreeStarPanel.SetActive(upgradeTo3Star);
        TwoStarPanel.SetActive(!upgradeTo3Star && upgradeTo2Star);

        CharacterLabelText.text = CharacterName;
    }


    public void OnClick()
    {
        gameManager = GameManager.Instance;

       // int unitCost = ((int)rareOptions) + 1;
        int unitCost = Economy.GetUnitCost(0, rareOptions);
        bool sold = gameManager.CanIBuy(unitCost);
        bool inventoryFree = !gameManager.InventoryIsFull();

        if (sold && inventoryFree)
        {
            gameManager.SubtractGold(unitCost);
            gameManager.SpawnUnitAtInventory(CharacterName);
        }
        else {
            Debug.LogError("sold" + sold + " cost " + unitCost + " invFree" + inventoryFree);
        }

    }
}
