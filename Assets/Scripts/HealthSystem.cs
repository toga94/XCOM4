using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour, IDamageable
{

    private float curHealth;
    private float healthMax;
    public bool isDie;
    public Action<float> OnHealthChanged;
    private Unit unit;
    private UnitObject unitObj;
    private void Start()
    {
        unit = GetComponent<Unit>();
        unitObj = unit.GetUnitObject;

        healthMax = unitObj.health;
        curHealth = healthMax;
    }


    public float Health
    {
        get
        {
            return curHealth;
        }
    }

    public void Damage(float value)
    {
        curHealth -= value;
        if (curHealth < 0) curHealth = 0;
        if(OnHealthChanged != null) OnHealthChanged(curHealth);
        Debug.Log("Damage  "+ Health);
    }

    public void Heal(float value)
    {
        curHealth += value;
        if (curHealth > healthMax) curHealth = healthMax;
        if(OnHealthChanged != null) OnHealthChanged(curHealth);
    }
}
