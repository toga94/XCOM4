using System;
using UnityEngine;

public class HealthSystem : IDamageable
{
    private float health;
    private float healthMax;
    public bool isDie;
    public EventHandler OnHealthChanged;
    public HealthSystem(float health)
    {
        this.health = health;
        healthMax = health;
    }

    public float Health
    {
        get
        {
            return health;
        }
    }

    public void Damage(float value)
    {
        health -= value;
        if (health < 0) health = 0;
        if(OnHealthChanged != null) OnHealthChanged(this, EventArgs.Empty);
        Debug.Log("Damage  "+ Health);
    }

    public void Heal(float value)
    {
        health += value;
        if (health > healthMax) health = healthMax;
        if(OnHealthChanged != null) OnHealthChanged(this, EventArgs.Empty);
    }
}
