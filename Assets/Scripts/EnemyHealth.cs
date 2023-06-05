using System;
using UnityEngine;
using Lean.Pool;
public class EnemyHealth : MonoBehaviour, IDamageable, IPoolable
{
    public event Action<float, int, float> OnHealthChanged;
    public event Action<bool> OnEnemyDie;
    public event Action<bool, GameObject> OnDie;

    [SerializeField] private Animator animator;

    [SerializeField] private GameObject enemyWorldUIPrefab;
    private EnemyWorldUI enemyWorldUI;
    private GameObject canvas;
    private GameObject canvasBar;

    private float health;
    [SerializeField] private float startHealth;
    public float GetStartHealth => startHealth;


    public float GetHealth => health;
    private Enemy enemy;
    public void Heal(float value)
    {
        health += value;
    }
    private bool dead;


    public void TakeDamage(float value)
    {
        float damage = value;

        if (damage > 0)
        {
            health -= damage;
            OnHealthChanged?.Invoke(health, 0, startHealth);
        }
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
        enemy = GetComponent<Enemy>();
        enemy.isDead = false;
        animator.SetBool("dead", false);
        dead = false;


        canvas = GameObject.Find("BarCanvas");
        canvasBar = (GameObject)Instantiate(enemyWorldUIPrefab, canvas.transform);
        this.enemyWorldUI = canvasBar.GetComponent<EnemyWorldUI>();
        this.enemyWorldUI.SetRoot(transform, canvas);
        OnHealthChanged?.Invoke(health, 0, startHealth);
    }

    public void OnDespawn()
    {
        Destroy(canvasBar);

    }
}
