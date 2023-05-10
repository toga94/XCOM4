using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    internal bool isDead;
    private EnemyHealth enemyHealth;
    [SerializeField]
    GameObject[] enemies;
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
    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        enemyHealth.OnEnemyDie += Die;
    }
    private void Update()
    {

        if (enemyHealth.GetHealth > 0 && GameStateSystem.Instance.GetCurrentState() is CombatPhaseState) {
            CombatPhase();
        }

    }
    private GameObject DetermineTarget()
    {
        agent = GetComponent<NavMeshAgent>();
        enemies = GameObject.FindGameObjectsWithTag(targetTag);
        if (enemies.Length > 0)
        {
            GameObject nearestEnemy = FindNearestEnemy(enemies);
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
        enemies = GameObject.FindGameObjectsWithTag(targetTag);
        if (enemies.Length > 0)
        {
            GameObject nearestEnemy = FindNearestEnemy(enemies);
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

    private void CombatPhase()
    {
        if (agent == null) return;
        agent.isStopped = false;
        Vector3 destination = DetermineDestination();

     
        agent.SetDestination(destination);
        if (enemies.Length == 0) return;

        if (agent.remainingDistance < agent.stoppingDistance && agent.velocity.magnitude < 0.3f)
        {
            if (Time.time - lastAttackTime >= attackDelay)
            {
                animator.SetFloat("run", 0);
                Attack(target);
                lastAttackTime = Time.time;
            }

        }
        else {
            animator.SetFloat("run", agent.velocity.magnitude / agent.speed);
        }
    }

    private void Attack(GameObject target)
    {
        // Calculate the direction to the target
        Vector3 direction = (target.transform.position - transform.position).normalized;

        // Calculate the rotation to face the target
        Quaternion to_Target_Quaternion = Quaternion.LookRotation(direction, Vector3.up);
        IDamageable damagableTarget = target.GetComponent<IDamageable>();

        if (damagableTarget == null) return;
        animator.SetTrigger("attack");
        damagableTarget.TakeDamage(enemyDamage);
    }
    void Die(bool value)
    {
        isDead = value;
    }
}
