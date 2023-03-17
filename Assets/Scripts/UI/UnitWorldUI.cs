using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Unit unit;

    private void Awake()
    {
        levelText = GetComponentInChildren<TextMeshProUGUI>();
        unit = transform.root.GetComponent<Unit>();
    }
    private void Update() {
        levelText.text = unit.GetUnitLevel.ToString();
    }

}
