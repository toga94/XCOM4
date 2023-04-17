using System.Collections;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using Lean.Pool;
using System.Collections.Generic;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Unit unit;
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private LeanGameObjectPool hpLinePool;
    private Transform root;
    private UnityEngine.Object hplineRes;
    [SerializeField] private Image hpSldier;
    [SerializeField] private Image hpDamageSldier;
    [SerializeField] private bool is3D;
    [SerializeField] private HorizontalLayoutGroup horizontalLayoutGroup;
    private int healthPerBar = 50;
    private bool uiInit;
    private float _healthBarOffsetZ;
    private Camera mainCamera;
    public float HealthBarOffsetZ;
    public float HealthBarOffsetYPercent = 10f;
    private List<GameObject> hpLine;
    private Canvas canvas;

    private void Awake()
    {


    }
    public void SetRoot(Transform value, GameObject canvasObj)
    {
        canvas = canvasObj.GetComponent<Canvas>();
        if (is3D)
        {
            root = transform.root;
        }
        else
        {
            root = value;
        }
        unit = root.GetComponent<Unit>();
        healthSystem = root.GetComponent<HealthSystem>();
        uiInit = true;
    }
    private void Start()
    {
        mainCamera = Camera.main;
        hplineRes = Resources.Load("hpLine");
        levelText = GetComponentInChildren<TextMeshProUGUI>();
        hpLinePool = GetComponent<LeanGameObjectPool>();
        hpLine = new List<GameObject>();

        healthSystem.OnHealthChanged += UpdateHp;
        UpdateHp(unit.GetUnitObject.health);
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += UpdateText;
        LevelGrid.Instance.OnAnyUnitSwappedGridPosition += UpdateText;

        InventoryGrid.Instance.OnAnyUnitMovedInventoryPosition += UpdateText;
        InventoryGrid.Instance.OnAnyUnitSwappedInventoryPosition += UpdateText;

        //HealthBarOffsetZ = 7f;
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
        if (!uiInit) return;


        float value = Mathf.Clamp(curHp / unit.GetUnitObject.health, 0, 1);
        hpSldier.fillAmount = value;


        int numBars = unit.GetUnitObject.health / healthPerBar;


        horizontalLayoutGroup.CalculateLayoutInputHorizontal();
        while (hpLinePool.Spawned < numBars)
        {
            hpLine.Clear();
            GameObject item = hpLinePool.Spawn(hpSldier.transform);
            hpLine.Add(item);
            item.transform.rotation = Quaternion.identity;
            item.transform.position = new Vector3(transform.position.x, 0, 0);
        }
        levelText.text = unit.GetUnitLevel.ToString();
    }

    private void Update()
    {
        if (hpDamageSldier.fillAmount != hpSldier.fillAmount)
        {
            hpDamageSldier.fillAmount = Mathf.Lerp(hpDamageSldier.fillAmount, hpSldier.fillAmount, Time.deltaTime * 2f);
        }
        Vector3 headPosition = unit.transform.position;

        // Calculate the position of the health bar in screen space
        Vector3 healthBarPosition = mainCamera.WorldToScreenPoint(headPosition);
        healthBarPosition.y += HealthBarOffsetZ;
        healthBarPosition.z = 0;

        // Set the position of the health bar
        transform.position = healthBarPosition;
    }


    private IEnumerator UpdateElement()
    {
        yield return new WaitForSeconds(0.015f);
        levelText.text = unit.GetUnitLevel.ToString();
        int numBars = unit.GetUnitObject.health / healthPerBar;
        foreach (var item in hpLine) { 
            item.transform.rotation = Quaternion.identity;
            item.transform.position = new Vector3(transform.position.x, 0, 0);
        }
        horizontalLayoutGroup.enabled = false;
        yield return new WaitForSeconds(0.015f);
        horizontalLayoutGroup.enabled = true;
        horizontalLayoutGroup.CalculateLayoutInputHorizontal();
    }
}
