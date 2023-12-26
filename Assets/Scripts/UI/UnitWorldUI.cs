using System.Collections;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using Lean.Pool;
using System.Collections.Generic;
using System.Linq;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Unit unit;
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private LeanGameObjectPool hpLinePool;
    private Transform root;
    [SerializeField] private Image hpSldier;

    [SerializeField] private Sprite hpPlayerSprite;
    [SerializeField] private Sprite hpEnemySprite;


    [SerializeField] private Image manaSldier;
    [SerializeField] private Image hpDamageSldier;
    [SerializeField] private bool is3D;
    [SerializeField] private HorizontalLayoutGroup horizontalLayoutGroup;
    [SerializeField] private int healthPerBar = 100;
    private bool uiInit;
    private Camera mainCamera;
    public float HealthBarOffsetZ;
    public float HealthBarOffsetYPercent = 10f;
    private bool isOwn;
    private List<GameObject> hpLine = new List<GameObject>();
    private Canvas canvas;
    private RectTransform rectTransform;
    private string curLevel;
    private float maxHp;
    private Transform unitTransform;
    public Vector3 offset;
    private float delay = 2f; // 2 seconds delay
    private float timeElapsed;
    private RectTransform canvasRect;

    public void SetRoot(Transform value, GameObject canvasObj, bool isOwn)
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
        this.isOwn = isOwn;
        if (isOwn)
        {
            hpSldier.sprite = hpPlayerSprite;
        }
        else {
            hpSldier.sprite = hpEnemySprite;
        }


            uiInit = true;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        levelText = GetComponentInChildren<TextMeshProUGUI>();
        hpLinePool = GetComponent<LeanGameObjectPool>();
        canvasRect = canvas.GetComponent<RectTransform>();
        healthSystem.OnHealthChanged += UpdateHp;
        healthSystem.OnManaChanged += UpdateMana;
        unitTransform = unit.transform;
        rectTransform = GetComponent<RectTransform>();

        
    }

    private void UpdateHp(float curHp, int level, float maxhp)
    {
        if (!uiInit || !hpSldier) return;

        float value = Mathf.Clamp(curHp / maxhp, 0, 1);
        hpSldier.fillAmount = value;

        int numBars = Mathf.FloorToInt(maxhp) / healthPerBar;

        maxHp = maxhp;
        if (horizontalLayoutGroup != null) horizontalLayoutGroup?.CalculateLayoutInputHorizontal();

        int numSpawnedBars = hpLine.Count;
        while (numSpawnedBars < numBars)
        {
            GameObject item = hpLinePool.Spawn(hpSldier.transform);
            hpLine.Add(item);
            item.transform.SetParent(hpSldier.transform, false);
            numSpawnedBars++;
        }

        curLevel = level.ToString();
        levelText.text = curLevel;
    }

    private void UpdateMana(float curMana, float maxMana)
    {
        if (!uiInit) return;

        float value = Mathf.Clamp(curMana / maxMana, 0, 1);
        manaSldier.fillAmount = value;
        levelText.text = curLevel;
    }

    private void Update()
    {
        if (hpDamageSldier.fillAmount != hpSldier.fillAmount)
        {
            hpDamageSldier.fillAmount = Mathf.Lerp(hpDamageSldier.fillAmount, hpSldier.fillAmount, Time.deltaTime * 2f);
        }

        Vector3 headPosition = unitTransform.position;
        rectTransform.anchoredPosition = WorldToCanvasPosition(canvasRect, mainCamera, headPosition + offset);

        UpdateElement();

        timeElapsed += Time.deltaTime;
        if (timeElapsed >= delay)
        {
            FixHpLine();
            timeElapsed = 0f;
        }
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
        float curHp = healthSystem.Health;
        int level = unit.GetUnitLevel;
        float maxHp = healthSystem.HealthMax;

        float hpValue = Mathf.Clamp(curHp / maxHp, 0, 1);
        hpSldier.fillAmount = hpValue;
        hpDamageSldier.fillAmount = Mathf.Lerp(hpDamageSldier.fillAmount, hpValue, Time.deltaTime * 2f);

        int numBars = Mathf.FloorToInt(maxHp) / healthPerBar;
        int numSpawnedBars = hpLine.Count;

        while (numSpawnedBars < numBars)
        {
            GameObject item = hpLinePool.Spawn(hpSldier.transform);
            hpLine.Add(item);
            item.transform.SetParent(hpSldier.transform, false);
            numSpawnedBars++;
        }

        curLevel = level.ToString();
        levelText.text = curLevel;
    }

    private void FixHpLine()
    {
        int numBars = Mathf.FloorToInt(maxHp) / healthPerBar;

        for (int i = 0; i < hpLine.Count; i++)
        {
            GameObject item = hpLine[i];
            item.transform.SetPositionAndRotation(
                new Vector3(transform.position.x, 0, 0),
                Quaternion.identity);
        }

        if (horizontalLayoutGroup != null)
        {
            horizontalLayoutGroup.enabled = false;
            horizontalLayoutGroup.enabled = true;
            horizontalLayoutGroup.CalculateLayoutInputHorizontal();
        }
    }
}
