using Lean.Pool;
using System;
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
    private void Start()
    {
        Economy.OnGoldChanged += UpdateUI;
        InventoryGrid.Instance.OnAnyUnitMovedInventoryPosition += UpdateUI;
        InventoryGrid.Instance.OnAnyUnitSwappedInventoryPosition += UpdateUI;
        InventoryGrid.Instance.OnAnyUnitAddedInventoryPosition += UpdateUI;

    }
    private void UpdateUI(object sender, EventArgs e)
    {
        CheckUpgrade();
    }
    private void UpdateUI(object sender, Unit e)
    {
        CheckUpgrade();
    }
    private void UpdateUI(int value)
    {
        Invoke(nameof(CheckUpgrade), 0.1f);
    }

    public void CheckUpgrade()
    {
        bool upgradeTo3Star = gameManager.CanIUpgradeTo3Star(unit);
        bool upgradeTo2Star = gameManager.CanIUpgradeTo2Star(unit);

        TreeStarPanel.SetActive(upgradeTo3Star);
        TwoStarPanel.SetActive(!upgradeTo3Star && upgradeTo2Star);
        HaveStarPanel.SetActive(upgradeTo3Star || upgradeTo2Star);
    }



    public void OnClick()
    {
        gameManager = GameManager.Instance;
        bool inventoryFree = !gameManager.InventoryIsFull();

        if (inventoryFree)
        {
            if (Economy.BuyUnit(unit))
                gameManager.SpawnUnitAtInventory(CharacterName);
        }
        else
        {
            Debug.LogError(" invFree" + inventoryFree);
        }
    }
}
