using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{

    public event Action<bool> OnEnemyDie;

    [SerializeField] private Animator animator;
    [SerializeField] private float health;
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
        OnEnemyDie?.Invoke(true);
        Destroy(gameObject, 3);
    }

}
