using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButtonUI : MonoBehaviour
{
    private Button button;
    [SerializeField]
    private Text levelText;
    [SerializeField]
    private Text costText;
    [SerializeField]
    private TextMeshProUGUI minMaxExp;
    [SerializeField]
    private Image xpBar;


    [SerializeField] private float fillRatio;
    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        EconomyManager.OnExperienceChanged += UpdateUI;
        UpdateUI(0);
    }
    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClick);
    }

    private void UpdateUI(int xp) {


        int level = EconomyManager.Level;
        int xpCost = EconomyManager.xpCost;
        costText.text = xpCost.ToString();
        levelText.text = $"<size=24>Level:</size> <size=32>{level.ToString()}</size>";

        minMaxExp.text = $"{EconomyManager.Exp}/{EconomyManager.GetExperienceNeededForNextLevel()} ";
        fillRatio = (float)(EconomyManager.Exp * 100  / EconomyManager.GetExperienceNeededForNextLevel()) / 100 ;
        xpBar.fillAmount = fillRatio;
    }

    public void OnClick()
    {
        int xpCost = EconomyManager.xpCost;
        if (!EconomyManager.CanIBuy(xpCost)) return;
        EconomyManager.SubtractGold(xpCost);
        EconomyManager.GainExperience(1);
    }
}
