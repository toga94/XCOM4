using Lean.Pool;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
        if (isUIUpdateNeeded && !isSoldOut)
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

        if ((upgradeTo3Star || upgradeTo2Star) && !isSoldOut)
        {
            // Gentle shake effect on HaveStarPanel with scaling
            Sequence shakeSequence = DOTween.Sequence();
            shakeSequence.Append(HaveStarPanel.transform.DOShakePosition(0.5f, 5f, 10, 90, false, true)) // Reduced shake strength to 5
                         .Join(HaveStarPanel.transform.DOScale(Vector3.one * 1.05f, 0.5f).SetEase(Ease.OutBack)) // Slight scale up to 1.05
                         .Append(HaveStarPanel.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack)); // Scale back to original size

            shakeSequence.SetLoops(-1, LoopType.Restart); // Loop the shake
        }
        else
        {
            // If sold out or no upgrade available, stop shaking
            HaveStarPanel.transform.DOKill(); // Stop any ongoing shakes
            HaveStarPanel.transform.localScale = Vector3.one; // Reset scale to original size
        }
    }
    public void ReEnable()
    {
        unitcardbutton.enabled = true;
        foreach (var item in disableui)
        {
            item.SetActive(true);
        }
        soldout.SetActive(false);
    }
    private bool isSoldOut;

    public void OnClick()
    {
        gameManager = GameManager.Instance;
        bool inventoryFree = !gameManager.InventoryIsFull();

        if (inventoryFree)
        {
            if (EconomyManager.BuyUnit(unit))
            {
                isUIUpdateNeeded = false;
                gameManager.SpawnUnitAtInventory(CharacterName, true);
                unitcardbutton.enabled = false;

                foreach (var item in disableui)
                {
                    item.SetActive(false);
                }

               

                // Manual self-shake effect on the button that was clicked
                Sequence shakeSequence = DOTween.Sequence();
                shakeSequence.Append(transform.DORotate(new Vector3(0, 0, 10), 0.1f).SetEase(Ease.OutBack))
                            .Append(transform.DORotate(new Vector3(0, 0, -10), 0.1f).SetEase(Ease.OutBack))
                            .Append(transform.DORotate(Vector3.zero, 0.1f).SetEase(Ease.OutBack));

                // Card flip animation
                Sequence flipSequence = DOTween.Sequence();
                flipSequence.Append(transform.DORotate(new Vector3(0, 180, 0), 0.3f).SetEase(Ease.OutBack)) // Flip to the back
                            .Join(transform.DOScale(Vector3.one * 1.1f, 0.3f)) // Slightly scale up during the flip
                            .Append(transform.DORotate(new Vector3(0, 360, 0), 0.3f).SetEase(Ease.OutBack)) // Flip back to the front
                            .Join(transform.DOScale(Vector3.one, 0.3f)).OnComplete(()=> {

                                // Enable soldout and trigger fade-in and shake effect
                                soldout.SetActive(true);

                                // Assuming soldout is a Text component for fade effect
                                Text soldoutText = soldout.GetComponent<Text>();

                                // Set initial alpha to 0 for fade-in effect
                                soldoutText.color = new Color(soldoutText.color.r, soldoutText.color.g, soldoutText.color.b, 0f);

                                // Fade in the Text component's color alpha from 0 to 1 over 0.3 seconds
                                soldoutText.DOFade(1f, 0.3f).SetEase(Ease.InOutQuad);

                                // Add scale up and down effect during fade-in for more impact
                                soldout.transform.localScale = Vector3.zero; // Set initial scale to 0 for dramatic effect
                                soldout.transform.DOScale(Vector3.one * 1.2f, 0.3f).SetEase(Ease.OutBack)
                                    .OnComplete(() => soldout.transform.DOScale(Vector3.one, 0.1f)); // Bounce back to original scale

                                // Shake the soldout GameObject
                                soldout.transform.DOShakePosition(0.6f, 20f, 15, 100, false, true);
                            }
                    ); // Scale back to original size

                isSoldOut = true;
            }
        }
        else
        {
            // Animation for the else case: Inventory is full
            Debug.LogError("Inventory is full");

            // Shake effect on the button to indicate the action can't be performed
            Sequence fullInventoryShake = DOTween.Sequence();
            fullInventoryShake.Append(transform.DORotate(new Vector3(0, 0, 10), 0.1f).SetEase(Ease.OutBack))
                              .Append(transform.DORotate(new Vector3(0, 0, -10), 0.1f).SetEase(Ease.OutBack))
                              .Append(transform.DORotate(Vector3.zero, 0.1f).SetEase(Ease.OutBack));

            // Optionally, you can also change the button's color briefly to indicate an error
            Color originalColor = unitcardbutton.image.color;
            unitcardbutton.image.DOColor(Color.red, 0.1f).OnComplete(() =>
                unitcardbutton.image.DOColor(originalColor, 0.1f)
            );
        }
    }


}
