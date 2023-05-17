using System;
using UnityEngine;
using Lean.Pool;
public class EnemyHealth : MonoBehaviour, IDamageable, IPoolable
{

    public event Action<bool> OnEnemyDie;
    public event Action<bool, GameObject> OnDie;

    [SerializeField] private Animator animator;
        
    private float health;
    [SerializeField] private float startHealth;
    public float GetHealth => health;

    public void Heal(float value)
    {
        health += value;
    }
    private bool dead;
    public void TakeDamage(float value)
    {
        health -= value;
        if (health <= 0 && !dead) {
            dead = true;
            Die();
        }
    }


    void Die()
    {
        animator.SetBool("dead", true);
        OnDie?.Invoke(true, this.gameObject);
        OnEnemyDie?.Invoke(true);
    }

    public void OnSpawn()
    {
        health = startHealth;
    }

    public void OnDespawn()
    {

    }
}
