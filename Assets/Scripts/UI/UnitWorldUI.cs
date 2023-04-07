using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Unit unit;
    [SerializeField] private HealthSystem healthSystem;

    private Transform root;

    [SerializeField] private RectTransform hpSldier;

    private void Awake()
    {
        root = transform.root;
        levelText = GetComponentInChildren<TextMeshProUGUI>();
        unit = root.GetComponent<Unit>();
        healthSystem = root.GetComponent<HealthSystem>();

    }

    private void Start()
    {
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += UpdateText;
        LevelGrid.Instance.OnAnyUnitSwappedGridPosition += UpdateText;

        InventoryGrid.Instance.OnAnyUnitMovedInventoryPosition += UpdateText;
        InventoryGrid.Instance.OnAnyUnitSwappedInventoryPosition += UpdateText;

        healthSystem.OnHealthChanged += UpdateHp;
        UpdateHp(100);
    }
    private void OnDisable()
    {
        LevelGrid.Instance.OnAnyUnitMovedGridPosition -= UpdateText;
        LevelGrid.Instance.OnAnyUnitSwappedGridPosition -= UpdateText;

        InventoryGrid.Instance.OnAnyUnitMovedInventoryPosition -= UpdateText;
        InventoryGrid.Instance.OnAnyUnitSwappedInventoryPosition -= UpdateText;

        healthSystem.OnHealthChanged -= UpdateHp;
    }
    private void UpdateText(object sender, EventArgs e)
    {
        StartCoroutine(UpdateElement());
    }

    private void UpdateHp(float curHp)
    {
        float value = Mathf.Clamp( curHp / unit.GetUnitObject.health , 0 , 1);
        hpSldier.localScale = new Vector3(value, 1, 1);
    }
    private IEnumerator UpdateElement() {
        yield return new WaitForSeconds(0.013f);
        levelText.text = unit.GetUnitLevel.ToString();
    }
}
