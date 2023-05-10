using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{

    public event Action<bool> OnEnemyDie;

    [SerializeField] private Animator animator;
    [SerializeField] private float health;

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

        Destroy(gameObject, 3);
    }
    private void OnDestroy()
    {
        OnEnemyDie?.Invoke(true);
    }
}
