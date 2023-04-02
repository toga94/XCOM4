using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Unit unit;

    private void Awake()
    {
        levelText = GetComponentInChildren<TextMeshProUGUI>();
        unit = transform.root.GetComponent<Unit>();
    }

    private void OnEnable()
    {
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += UpdateText;
        LevelGrid.Instance.OnAnyUnitSwappedGridPosition += UpdateText;

        InventoryGrid.Instance.OnAnyUnitMovedInventoryPosition += UpdateText;
        InventoryGrid.Instance.OnAnyUnitSwappedInventoryPosition += UpdateText;
    }
    private void OnDisable()
    {
        LevelGrid.Instance.OnAnyUnitMovedGridPosition -= UpdateText;
        LevelGrid.Instance.OnAnyUnitSwappedGridPosition -= UpdateText;

        InventoryGrid.Instance.OnAnyUnitMovedInventoryPosition -= UpdateText;
        InventoryGrid.Instance.OnAnyUnitSwappedInventoryPosition -= UpdateText;
    }
    private void UpdateText(object sender, EventArgs e)
    {
        StartCoroutine(UpdateElement());
    }


    private IEnumerator UpdateElement() {
        yield return new WaitForSeconds(0.013f);
        levelText.text = unit.GetUnitLevel.ToString();
    }
}
