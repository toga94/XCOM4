using System;
using UnityEngine;
using Lean.Pool;
using MoreMountains.Feedbacks;

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

    private MMF_Player mmf_player;
    public float GetHealth => health;
    private Enemy enemy;
    public void Heal(float value)
    {
        health += value;
    }
    private bool dead;

    private void Start()
    {
        mmf_player = GameManager.Instance.GetMMF_Player;
    }

    public void TakeDamage(float value, bool isCritical)
    {
        float damage = value;

        if (damage > 0)
        {
            health -= damage;
            OnHealthChanged?.Invoke(health, 0, startHealth);

            SetFloatingTextProperties(value, isCritical);

            mmf_player?.PlayFeedbacks(enemy.transform.position + Vector3.up * 9, damage);
        }
        if (health <= 0 && !dead) {
            dead = true;
            Die();
        }

    }
    private void SetFloatingTextProperties(float damage, bool isCritical)
    {
        MMF_FloatingText text = mmf_player.GetFeedbackOfType<MMF_FloatingText>();
        Gradient gradient = new Gradient();
        gradient.colorKeys = new GradientColorKey[]
        {
            new GradientColorKey(Color.red, 0f),    // Start color (0%)
            new GradientColorKey(Color.white, 1f)   // End color (100%)
        };
        text.AnimateColorGradient = gradient;
        text.ForceColor = isCritical;
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
