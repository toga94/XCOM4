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
    public GameObject HaveStarPanel;
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
        HaveStarPanel.SetActive(upgradeTo3Star || upgradeTo2Star);

        CharacterLabelText.text = CharacterName;
    }


    public void OnClick()
    {
        gameManager = GameManager.Instance;
        bool inventoryFree = !gameManager.InventoryIsFull();

        if (inventoryFree)
        {
            if(Economy.BuyUnit(unit))
                gameManager.SpawnUnitAtInventory(CharacterName);

        }
        else {
          Debug.LogError(" invFree" + inventoryFree);
        }

    }
}
