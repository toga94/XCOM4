using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    internal bool isDead;
    private EnemyHealth enemyHealth;
    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        enemyHealth.OnEnemyDie += Die;
    }

    // Update is called once per frame
    void Die(bool value)
    {
        isDead = value;
    }
}
