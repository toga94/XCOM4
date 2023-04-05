using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyUI : MonoBehaviour
{
    private Text goldText;

    private void OnDisable()
    {
       // Economy.OnGoldChanged -= UpdateGoldText;
    }
    private void Start()
    {
        GameManager gm = GameManager.Instance;
        Economy.OnGoldChanged += UpdateGoldText;
        goldText = GetComponent<Text>();
        goldText.text = Economy.GetGold().ToString();
    }
    private void UpdateGoldText(int gold)
    {
        goldText.text = gold.ToString();
    }


}
