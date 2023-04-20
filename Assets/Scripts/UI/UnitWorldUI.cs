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
    private RectTransform rectTransform;
    private string curLevel;
    private float maxHp;
    private Transform unitTransform;
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
        maxHp = unit.MaxHealth;
        curLevel = unit.GetUnitLevel.ToString();
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
        canvasRect = canvas.GetComponent<RectTransform>();
        healthSystem.OnHealthChanged += UpdateHp;



        unitTransform = unit.transform;


        rectTransform = GetComponent<RectTransform>();
    }



    private void UpdateHp(float curHp, int level, float maxhp)
    {
        if (!uiInit) return;


        float value = Mathf.Clamp(curHp / maxhp, 0, 1);
        hpSldier.fillAmount = value;


        int numBars = Mathf.FloorToInt(maxhp) / healthPerBar;

        maxHp = maxhp;
        horizontalLayoutGroup.CalculateLayoutInputHorizontal();
        while (hpLinePool.Spawned < numBars)
        {
            hpLine.Clear();
            GameObject item = hpLinePool.Spawn(hpSldier.transform);
            hpLine.Add(item);
            item.transform.SetPositionAndRotation(
                new Vector3(transform.position.x, 0, 0),
                Quaternion.identity);
        }
        curLevel = level.ToString();
        levelText.text = curLevel;
    }
    public Vector3 offset;
    RectTransform canvasRect;
    private void Update()
    {
        if (hpDamageSldier.fillAmount != hpSldier.fillAmount)
        {
            hpDamageSldier.fillAmount = Mathf.Lerp(hpDamageSldier.fillAmount, hpSldier.fillAmount, Time.deltaTime * 2f);
        }
        Vector3 headPosition = unitTransform.position;

        rectTransform.anchoredPosition = WorldToCanvasPosition(canvasRect, mainCamera, headPosition + offset);

        UpdateElement();
    }

    private Vector2 WorldToCanvasPosition(RectTransform canvas, Camera camera, Vector3 position)
    {
        Vector2 temp = camera.WorldToViewportPoint(position);

        temp.x *= canvas.sizeDelta.x;
        temp.y *= canvas.sizeDelta.y;

        temp.x -= canvas.sizeDelta.x * canvas.pivot.x;
        temp.y -= canvas.sizeDelta.y * canvas.pivot.y;

        return temp;
    }
    private void UpdateElement()
    {

        levelText.text = curLevel;
        int numBars =  Mathf.FloorToInt(maxHp) / healthPerBar;
        foreach (var item in hpLine)
        {
            item.transform.SetPositionAndRotation(
                new Vector3(transform.position.x, 0, 0),
                Quaternion.identity);
        }
        horizontalLayoutGroup.enabled = false;

        horizontalLayoutGroup.enabled = true;
        horizontalLayoutGroup.CalculateLayoutInputHorizontal();

        UpdateHp(healthSystem.Health, unit.GetUnitLevel, healthSystem.HealthMax);
    }
}
