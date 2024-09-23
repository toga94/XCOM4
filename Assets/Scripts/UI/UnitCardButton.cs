using Lean.Pool;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UnitCardButton : MonoBehaviour, IPoolable
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
    private Coroutine checkUpgradeCoroutine;
    InventoryGrid inventoryGrid;
    private bool isUIUpdateNeeded = true;
    [SerializeField] private GameObject[] disableui;
    [SerializeField] private GameObject soldout;
    [SerializeField] private Button unitcardbutton;


    public void OnSpawn()
    {
        gameManager = GameManager.Instance;
        inventoryGrid = InventoryGrid.Instance;

        EconomyManager.OnGoldChanged += UpdateUI;
        inventoryGrid.OnAnyUnitMovedInventoryPosition += UpdateUI;
        inventoryGrid.OnAnyUnitSwappedInventoryPosition += UpdateUI;
        inventoryGrid.OnAnyUnitAddedInventoryPosition += UpdateUI;
    }

    public void OnDespawn()
    {

    }

    private void UpdateUI(object sender, EventArgs e)
    {
        isUIUpdateNeeded = true;
    }

    private void UpdateUI(object sender, Unit e)
    {
        isUIUpdateNeeded = true;
    }

    private void UpdateUI(int value)
    {
        isUIUpdateNeeded = true;
    }

    private void LateUpdate()
    {
        if (isUIUpdateNeeded)
        {
            CheckUpgrade();
        }
    }

    public void CheckUpgrade()
    {
        bool upgradeTo3Star = gameManager.CanIUpgradeTo3Star(unit);
        bool upgradeTo2Star = gameManager.CanIUpgradeTo2Star(unit);
        
        TreeStarPanel.SetActive(upgradeTo3Star);
        TwoStarPanel.SetActive(!upgradeTo3Star && upgradeTo2Star);
        HaveStarPanel.SetActive(upgradeTo3Star || upgradeTo2Star);

        isUIUpdateNeeded = false;
    }

    public void ReEnable() {
        unitcardbutton.enabled = true;
        foreach (var item in disableui)
        {
            item.SetActive(true);
        }
        soldout.SetActive(false);
    }

    public void OnClick()
    {
        gameManager = GameManager.Instance;
        bool inventoryFree = !gameManager.InventoryIsFull();

        if (inventoryFree)
        {
            if (EconomyManager.BuyUnit(unit)) {
                gameManager.SpawnUnitAtInventory(CharacterName, true);
                unitcardbutton.enabled = false;
                foreach (var item in disableui)
                {
                    item.SetActive(false);
                }
                soldout.SetActive(true);
            }
              
        }
        else
        {
            Debug.LogError(" invFree" + inventoryFree);
        }
    }


}
