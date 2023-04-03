using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyUI : MonoBehaviour
{
    private Text goldText;

    private void OnDisable()
    {
        GameManager.Instance.OnGoldChanged += UpdateGoldText;
    }
    private void Start()
    {
        GameManager gm = GameManager.Instance;
        gm.OnGoldChanged += UpdateGoldText;
        goldText = GetComponent<Text>();
        goldText.text = gm.GetGold.ToString();
    }
    private void UpdateGoldText(int gold)
    {
        goldText.text = gold.ToString();
    }


}
