using System.Collections;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using Lean.Pool;
public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Unit unit;
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private LeanGameObjectPool hpLinePool;
    private Transform root;
    private UnityEngine.Object hplineRes;
    [SerializeField] private RectTransform hpSldier;

    private void Awake()
    {
        root = transform.root;
        levelText = GetComponentInChildren<TextMeshProUGUI>();
        unit = root.GetComponent<Unit>();
        healthSystem = root.GetComponent<HealthSystem>();
        hpLinePool = GetComponent<LeanGameObjectPool>();
    }

    private void Start()
    {
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += UpdateText;
        LevelGrid.Instance.OnAnyUnitSwappedGridPosition += UpdateText;

        InventoryGrid.Instance.OnAnyUnitMovedInventoryPosition += UpdateText;
        InventoryGrid.Instance.OnAnyUnitSwappedInventoryPosition += UpdateText;

        hplineRes = Resources.Load("hpLine");

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

        int healthPerBar = 50; 
        int numBars = unit.GetUnitObject.health / healthPerBar;

        for (int i = 0; i < numBars; i++)
        {
            GameObject hpLine = hpLinePool.Spawn(hpSldier);
        }


     
    }

    private IEnumerator UpdateElement() {
        yield return new WaitForSeconds(0.013f);
        levelText.text = unit.GetUnitLevel.ToString();
    }
}
