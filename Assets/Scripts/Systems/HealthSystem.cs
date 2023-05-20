using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour, IDamageable
{
    public bool IsDie => Health <= 0;
    public event Action<float, int, float> OnHealthChanged;
    public event Action<float, float> OnManaChanged;
    public event Action<bool, GameObject> OnDie;

    


    private UnitObject unitObj;
    private Unit unit;
    private float health;
    private float healthMax;
    [SerializeField] private float mana;
    [SerializeField] private float manaMax;

    private GameObject canvasBar;
    private UnitWorldUI unitWorldUI;
    private GameObject canvas;
    [SerializeField] private GameObject unitWorldUIPrefab;
    private void Start()
    {
        unit = GetComponent<Unit>();
        unitObj = unit.GetUnitObject;
        int unitlevel = unit.GetUnitLevel;
        healthMax = unitObj.health * (unitlevel + 1);
        manaMax = unitObj.mana * (unitlevel + 1);
        mana = manaMax;
        health = healthMax;
        canvas = GameObject.Find("BarCanvas");
        canvasBar = (GameObject)Instantiate(unitWorldUIPrefab, canvas.transform);
        this.unitWorldUI = canvasBar.GetComponent<UnitWorldUI>();
        this.unitWorldUI.SetRoot(transform, canvas);
        OnHealthChanged?.Invoke(health, unit.GetUnitLevel, healthMax);
        OnManaChanged?.Invoke(mana, manaMax);

        GameStateSystem.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState obj)
    {
        int unitlevel = unit.GetUnitLevel;
        healthMax = unitObj.health * (unitlevel + 1);
        manaMax = unitObj.mana * (unitlevel + 1);

        if (obj.IsCombatState)
        {
            mana = 0;
        }
        else {
            mana = manaMax;
        }

        health = healthMax;

        Heal(999999);
    }

    public void TakeDamage(float value)
    {
        if (IsDie)
        {
            OnDie?.Invoke(true, gameObject);
        }
        else
        {
            float damage = value - unitObj.defence;
            if (damage > 0)
            {
                health = Mathf.Max(health - damage, 0);
                healthMax = unitObj.health * (unit.GetUnitLevel + 1);
                OnHealthChanged?.Invoke(health, unit.GetUnitLevel, healthMax);
            }
        }
    }
    public void DecreaseMana(float value)
    {
        mana -= value;
        if (mana > manaMax)
        {
            mana = manaMax;
            mana -= value;
        }
        if (mana < 0) mana = 0;

        OnManaChanged?.Invoke(mana, manaMax);
    }
    public void IncreaseMana(float value)
    {
        mana += value;
        if (mana > manaMax) mana = manaMax;
        float valueMax = Mathf.Clamp(GetMana / GetMaxMana, 0, 1);
    
        OnManaChanged?.Invoke(mana, manaMax);
    }
    public void Heal(float value)
    {
        healthMax = unitObj.health * (unit.GetUnitLevel + 1);
        health = Mathf.Min(health + value, healthMax);
        OnHealthChanged?.Invoke(health, unit.GetUnitLevel, healthMax);
    }

    public float Health => health;
    public float HealthMax => healthMax;

    public float GetMana => mana;
    public float GetMaxMana => manaMax;

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