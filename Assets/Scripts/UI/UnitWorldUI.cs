using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Lean.Pool;
using System.Collections.Generic;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Unit unit;
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private LeanGameObjectPool hpLinePool;
    [SerializeField] private Image hpSlider;
    [SerializeField] private Image manaSlider;
    [SerializeField] private Image hpDamageSlider;

    [SerializeField] private Sprite hpPlayerSprite;
    [SerializeField] private Sprite hpEnemySprite;

    [SerializeField] private bool is3D;
    [SerializeField] private HorizontalLayoutGroup horizontalLayoutGroup;
    [SerializeField] private int healthPerBar = 100;

    private Transform root;
    private bool uiInit;
    private Camera mainCamera;
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
    private bool isOwn;

    public void SetRoot(Transform value, GameObject canvasObj, bool isOwn)
    {
        canvas = canvasObj.GetComponent<Canvas>();
        root = is3D ? transform.root : value;

        unit = root.GetComponent<Unit>();
        healthSystem = root.GetComponent<HealthSystem>();

        maxHp = unit.MaxHealth;
        curLevel = unit.GetUnitLevel.ToString();
        this.isOwn = isOwn;

        hpSlider.sprite = isOwn ? hpPlayerSprite : hpEnemySprite;

        uiInit = true;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        levelText = GetComponentInChildren<TextMeshProUGUI>();
        hpLinePool = GetComponent<LeanGameObjectPool>();
        rectTransform = GetComponent<RectTransform>();
        canvasRect = canvas.GetComponent<RectTransform>();

        healthSystem.OnHealthChanged += UpdateHp;
        healthSystem.OnManaChanged += UpdateMana;

        unitTransform = unit.transform;
    }

    private void Update()
    {
        if (!uiInit) return;

        // Smooth damage slider
        UpdateDamageSlider();

        // Update UI position based on world position
        UpdateUIPosition();

        // Periodically fix HP bar alignment
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= delay)
        {
            FixHpLine();
            timeElapsed = 0f;
        }
    }

    private void UpdateHp(float curHp, int level, float maxhp)
    {
        if (!uiInit || !hpSlider) return;

        curLevel = level.ToString();
        levelText.text = curLevel;

        maxHp = maxhp;

        // Update HP UI
        UpdateHealthBar(curHp, maxhp);
        UpdateHealthBarsCount(maxhp);
    }

    private void UpdateMana(float curMana, float maxMana)
    {
        if (!uiInit || !manaSlider) return;

        float value = Mathf.Clamp(curMana / maxMana, 0, 1);
        manaSlider.fillAmount = value;
        levelText.text = curLevel;
    }

    private void UpdateHealthBar(float curHp, float maxHp)
    {
        float hpValue = Mathf.Clamp(curHp / maxHp, 0, 1);
        hpSlider.fillAmount = hpValue;
        hpDamageSlider.fillAmount = Mathf.Lerp(hpDamageSlider.fillAmount, hpValue, Time.deltaTime * 2f);
    }

    private void UpdateHealthBarsCount(float maxHp)
    {
        int numBars = Mathf.FloorToInt(maxHp) / healthPerBar;
        int numSpawnedBars = hpLine.Count;

        while (numSpawnedBars < numBars)
        {
            GameObject item = hpLinePool.Spawn(hpSlider.transform);
            hpLine.Add(item);
            item.transform.SetParent(hpSlider.transform, false);
            numSpawnedBars++;
        }
    }

    private void UpdateDamageSlider()
    {
        if (hpDamageSlider.fillAmount != hpSlider.fillAmount)
        {
            hpDamageSlider.fillAmount = Mathf.Lerp(hpDamageSlider.fillAmount, hpSlider.fillAmount, Time.deltaTime * 2f);
        }
    }

    private void UpdateUIPosition()
    {
        Vector3 headPosition = unitTransform.position;
        rectTransform.anchoredPosition = WorldToCanvasPosition(canvasRect, mainCamera, headPosition + offset);
    }

    private Vector2 WorldToCanvasPosition(RectTransform canvas, Camera camera, Vector3 position)
    {
        Vector2 viewportPos = camera.WorldToViewportPoint(position);
        viewportPos.x *= canvas.sizeDelta.x;
        viewportPos.y *= canvas.sizeDelta.y;
        viewportPos.x -= canvas.sizeDelta.x * canvas.pivot.x;
        viewportPos.y -= canvas.sizeDelta.y * canvas.pivot.y;
        return viewportPos;
    }

    private void FixHpLine()
    {
        int numBars = Mathf.FloorToInt(maxHp) / healthPerBar;

        foreach (var item in hpLine)
        {
            item.transform.SetPositionAndRotation(new Vector3(transform.position.x, 0, 0), Quaternion.identity);
        }

        if (horizontalLayoutGroup != null)
        {
            horizontalLayoutGroup.enabled = false;
            horizontalLayoutGroup.enabled = true;
            horizontalLayoutGroup.CalculateLayoutInputHorizontal();
        }
    }
}