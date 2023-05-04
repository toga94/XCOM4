using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    private Animator animator;
    float health;
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

}
