using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Lean.Pool;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    public bool isDead;
    private EnemyHealth enemyHealth;
    [SerializeField]
    private List<Unit> enemies;
    [SerializeField]
    private GameObject target;
    private GameObject currentTarget = null;
    private float attackRange = 1f;
    private float attackDelay = 1f;
    private float lastAttackTime = 0f;
    private string targetTag = "Unit";
    private GameObject targetObject;
    [SerializeField]
    private float enemyDamage;
    private GameStateSystem gameStateSystem;
    private GameManager gameManager;

    public LeanGameObjectPool objectPool;
    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        enemyHealth.OnEnemyDie += Die;
        gameStateSystem = GameStateSystem.Instance;
        gameManager = GameManager.Instance;
    }
    private void Update()
    {
        if (enemyHealth.GetHealth > 0 && gameStateSystem.GetCurrentState.IsCombatState) {
            CombatPhase();
        }
    }
    private GameObject DetermineTarget()
    {
        agent = GetComponent<NavMeshAgent>();
        enemies = gameManager.GetAllUnitsOnGrid;
        if (enemies.Count > 0)
        {
            GameObject nearestEnemy = FindNearestEnemyUnit(enemies);
            if (Vector3.Distance(transform.position, nearestEnemy.transform.position) <= attackRange)
            {
                return nearestEnemy;
            }
            else
            {
                return null;
            }
        }
        else
        {
            currentTarget = null;
            return null;
        }
    }
    private Vector3 DetermineDestination()
    {
        enemies = gameManager.GetAllUnitsOnGrid;
        if (enemies.Count > 0)
        {
            GameObject nearestEnemy = FindNearestEnemyUnit(enemies);
            target = nearestEnemy;
            return nearestEnemy.transform.position;
        }
        else
        {
            float range = 10f;
            Vector3 randomDirection = Random.insideUnitSphere * range;
            randomDirection += transform.position;
            NavMeshHit navMeshHit;
            NavMesh.SamplePosition(randomDirection, out navMeshHit, range, NavMesh.AllAreas);
            return navMeshHit.position;
        }
    }
    private GameObject FindNearestEnemy(GameObject[] enemies)
    {
        return enemies
            .OrderBy(enemy => Vector3.Distance(transform.position, enemy.transform.position))
            .FirstOrDefault();
    }
    private GameObject FindNearestEnemyUnit(List<Unit> enemies)
    {
        return enemies
            .OrderBy(enemy => Vector3.Distance(transform.position, enemy.transform.position))
            .Select(enemy => enemy.gameObject).FirstOrDefault();
    }

    private void CombatPhase()
    {
        if (agent == null) return;
        agent.isStopped = false;
        Vector3 destination = DetermineDestination();

     
        agent.SetDestination(destination);
        if (enemies.Count == 0) return;

        if (agent.remainingDistance < agent.stoppingDistance && agent.velocity.magnitude < 0.3f)
        {
            if (Time.time - lastAttackTime >= attackDelay)
            {
                animator.SetFloat("run", 0);
                if (agent.pathStatus == NavMeshPathStatus.PathComplete)
                {
                    try
                    {
                        if (target != null) Attack(target);
                        lastAttackTime = Time.time;
                    }

                    catch (System.Exception)
                    {

                    }
                }
            }

        }
        else {
            animator.SetFloat("run", agent.velocity.magnitude / agent.speed);
        }
    }

    private void Attack(GameObject target)
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        Quaternion to_Target_Quaternion = Quaternion.LookRotation(direction, Vector3.up);
        IDamageable damagableTarget = target.GetComponent<IDamageable>();

        if (damagableTarget == null) return;
        animator.SetTrigger("attack");
        damagableTarget.TakeDamage(enemyDamage);
    }
    void Die(bool value)
    {
        isDead = value;
        Destroy(gameObject, 3);
        //objectPool.Despawn(gameObject, 3);
    }
}
