using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour, IDamageable
{
    public bool IsDie => Health <= 0;
    public event Action<float, int, float> OnHealthChanged;

    private UnitObject unitObj;
    private Unit unit;
    private float health;
    private float healthMax;
    private float mana;
    private float manaMax;

    private GameObject canvasBar;
    private UnitWorldUI unitWorldUI;
    private GameObject canvas;
    [SerializeField] private GameObject unitWorldUIPrefab;
    private void Start()
    {
        unit = GetComponent<Unit>();
        unitObj = unit.GetUnitObject;
        healthMax = unitObj.health * (unit.GetUnitLevel + 1);
        health = healthMax;
        canvas = GameObject.Find("BarCanvas");
        //GameObject unitWorldUIPrefab = (GameObject) Resources.Load("UnitWorldUI2D");
        canvasBar = (GameObject)Instantiate(unitWorldUIPrefab, canvas.transform);
        this.unitWorldUI = canvasBar.GetComponent<UnitWorldUI>();
        this.unitWorldUI.SetRoot(transform, canvas);
        OnHealthChanged?.Invoke(health, unit.GetUnitLevel, healthMax);
    }
    public void TakeDamage(float value)
    {
        float damage = value - unitObj.defence;
        if (damage > 0)
        {
            health = Mathf.Max(health - damage, 0);
            healthMax = unitObj.health * (unit.GetUnitLevel + 1);
            OnHealthChanged?.Invoke(health, unit.GetUnitLevel, healthMax);
            Debug.Log($"Damage: {damage}, Health: {health}");
        }
    }
    public void DecreaseMana(float value)
    {
        mana -= value;
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



    private void OnDestroy()
    {
        Destroy(canvasBar);
    }
}