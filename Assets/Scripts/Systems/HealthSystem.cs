using System;
using UnityEngine;
using MoreMountains.Feedbacks;

public class HealthSystem : MonoBehaviour, IDamageable
{
    // Use properties instead of public fields
    public bool IsDie => Health <= 0;
    public float Health { get; private set; }
    public float HealthMax { get; private set; }
    public float GetMana { get; private set; }
    public float GetMaxMana { get; private set; }

    public event Action<float, int, float> OnHealthChanged;
    public event Action<float, float> OnManaChanged;
    public event Action<bool, GameObject> OnDie;

    private UnitObject unitObj;
    private Unit unit;
    [SerializeField] private float mana;
    [SerializeField] private float manaMax;

    private GameObject canvasBar;
    private UnitWorldUI unitWorldUI;
    private GameObject canvas;
    private GameStateSystem gameStateSystem;
    [SerializeField] private GameObject unitWorldUIPrefab;
    private MMF_Player mmf_player;

    private void Awake()
    {
        unit = GetComponent<Unit>();
        unitObj = unit.GetUnitObject;
        HealthMax = unitObj.health * (unit.GetUnitLevel + 1);
        manaMax = unitObj.mana * (unit.GetUnitLevel + 1);
        GetMana = manaMax;
        Health = HealthMax;
    }

    private void Start()
    {
        InitializeUI();
        gameStateSystem = GameStateSystem.Instance;
        gameStateSystem.OnGameStateChanged += OnGameStateChanged;
        mmf_player = GameManager.Instance.GetMMF_Player;
    }

    private void InitializeUI()
    {
        canvas = GameObject.Find("BarCanvas");
        canvasBar = Instantiate(unitWorldUIPrefab, canvas.transform);
        unitWorldUI = canvasBar.GetComponent<UnitWorldUI>();
        unitWorldUI.SetRoot(transform, canvas, unit.isOwn);
        OnHealthChanged?.Invoke(Health, unit.GetUnitLevel, HealthMax);
        OnManaChanged?.Invoke(GetMana, manaMax);
    }

    private void OnGameStateChanged(GameState obj)
    {
        int unitlevel = unit.GetUnitLevel;
        HealthMax = unitObj.health * (unitlevel + 1);
        manaMax = unitObj.mana * (unitlevel + 1);

        if (obj.IsCombatState)
        {
            GetMana = 0;
        }
        else
        {
            GetMana = manaMax;
        }

        Health = HealthMax;

        Heal(999999);
    }

    public void TakeDamage(float value, bool isCritical)
    {
        if (GameStateSystem.Instance.CurrentState is ChampionSelectionState) return;

        if (IsDie)
        {
            OnDie?.Invoke(true, gameObject);
        }
        else
        {
            float damage = value - unitObj.defence;
            if (damage > 0)
            {
                Health = Mathf.Max(Health - damage, 0);
                HealthMax = unitObj.health * (unit.GetUnitLevel + 1);
                OnHealthChanged?.Invoke(Health, unit.GetUnitLevel, HealthMax);
            }

            SetFloatingTextProperties(value, isCritical);

            mmf_player?.PlayFeedbacks(unit.UnitPosition + Vector3.up * 9, damage);
        }
    }

    private void SetFloatingTextProperties(float damage, bool isCritical)
    {
        MMF_FloatingText text = mmf_player.GetFeedbackOfType<MMF_FloatingText>();
        Gradient gradient = new Gradient();
        gradient.colorKeys = new GradientColorKey[]
        {
            new GradientColorKey(Color.red, 0f),    // Start color (0%)
            new GradientColorKey(Color.white, 1f)   // End color (100%)
        };
        text.AnimateColorGradient = gradient;
        text.ForceColor = isCritical;
    }

    public void DecreaseMana(float value)
    {
        GetMana -= value;
        GetMana = Mathf.Clamp(GetMana, 0, manaMax);
        OnManaChanged?.Invoke(GetMana, manaMax);
    }

    public void IncreaseMana(float value)
    {
        GetMana += value;
        GetMana = Mathf.Clamp(GetMana, 0, manaMax);
        OnManaChanged?.Invoke(GetMana, manaMax);
    }

    public void Heal(float value)
    {
        HealthMax = unitObj.health * (unit.GetUnitLevel + 1);
        Health = Mathf.Min(Health + value, HealthMax);
        OnHealthChanged?.Invoke(Health, unit.GetUnitLevel, HealthMax);
    }

    private void OnDisable()
    {
        if (canvasBar)
            canvasBar.SetActive(false);
    }

    private void OnEnable()
    {
        if (canvasBar)
            canvasBar.SetActive(true);
    }

    private void OnDestroy()
    {
        Destroy(canvasBar);
    }
}
