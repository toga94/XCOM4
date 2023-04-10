using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour, IDamageable
{
    public bool IsDie => Health <= 0;
    public event Action<float> OnHealthChanged;

    private UnitObject unitObj;
    private Unit unit;
    private float health;
    private float healthMax;

    private void Start()
    {
        unit = GetComponent<Unit>();
        unitObj = unit.GetUnitObject;
        healthMax = unitObj.health * (unit.GetUnitLevel + 1);
        health = healthMax;
    }

    public void Damage(float value)
    {
        float damage = value - unitObj.defence;
        if (damage > 0)
        {
            health = Mathf.Max(health - damage, 0);
            OnHealthChanged?.Invoke(health);
            Debug.Log($"Damage: {damage}, Health: {health}");
        }
    }

    public void Heal(float value)
    {
        health = Mathf.Min(health + value, healthMax);
        OnHealthChanged?.Invoke(health);
    }

    public float Health => health;
}