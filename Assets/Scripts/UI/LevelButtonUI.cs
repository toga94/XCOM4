using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonUI : MonoBehaviour
{
    private Button button;
    [SerializeField]private Text levelText;
    [SerializeField]private Text costText;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        Economy.OnExperienceChanged += UpdateUI;
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
    }

    public void OnClick()
    {
        int xpCost = Economy.xpCost;
        if (!Economy.CanIBuy(xpCost)) return;
        Economy.SubtractGold(xpCost);
    }
}
