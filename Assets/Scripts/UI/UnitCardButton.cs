using System;
using UnityEngine;
using UnityEngine.UI;

public class UnitCardButton : MonoBehaviour
{
    public event Action<UnitCardButton> OnClicked;
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

    private void Update()
    {
        bool upgradeTo3Star = gameManager.CanIUpgradeTo3Star(unit);
        bool upgradeTo2Star = gameManager.CanIUpgradeTo2Star(unit);
        CharacterLabelText.text = CharacterName;
        TreeStarPanel.SetActive(upgradeTo3Star);
        TwoStarPanel.SetActive(!upgradeTo3Star && upgradeTo2Star);
        HaveStarPanel.SetActive(upgradeTo3Star || upgradeTo2Star);
    }

    public void OnClick()
    {
        OnClicked?.Invoke(this);
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
