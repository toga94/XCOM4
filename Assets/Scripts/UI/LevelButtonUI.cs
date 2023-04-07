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
        Economy.OnExperienceChanged += UpdateUI;
        UpdateUI(0);
    }
    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClick);
    }

    private void UpdateUI(int xp) {


        int level = Economy.Level;
        int xpCost = Economy.xpCost;
        costText.text = xpCost.ToString();
        levelText.text = level.ToString();

        minMaxExp.text = $"{Economy.Exp}/{Economy.GetExperienceNeededForNextLevel()} ";
        fillRatio = (float)(Economy.Exp * 100  / Economy.GetExperienceNeededForNextLevel()) / 100 ;
        xpBar.fillAmount = fillRatio;
    }

    public void OnClick()
    {
        int xpCost = Economy.xpCost;
        if (!Economy.CanIBuy(xpCost)) return;
        Economy.SubtractGold(xpCost);
        Economy.GainExperience(1);
    }
}
