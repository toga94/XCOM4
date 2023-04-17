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

    [SerializeField] private GameObject canvasBar;
    [SerializeField] private UnitWorldUI unitWorldUI;
    [SerializeField] private GameObject canvas;

    private void Start()
    {
        unit = GetComponent<Unit>();
        unitObj = unit.GetUnitObject;
        healthMax = unitObj.health * (unit.GetUnitLevel + 1);
        health = healthMax;
        canvas = GameObject.Find("Canvas");
        canvasBar = (GameObject) Instantiate(Resources.Load("UnitWorldUI2D"), canvas.transform);
        unitWorldUI = canvasBar.GetComponent<UnitWorldUI>();
        unitWorldUI.SetRoot( transform , canvas);
    }

    public void TakeDamage(float value)
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


    private void Update()
    {
        
    }

    private void OnDestroy()
    {
        Destroy(canvasBar);
    }
}