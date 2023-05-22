using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
public class UnitAI : MonoBehaviour
{

    private Unit unit;
    [SerializeField]
    private UnitObject unitObject;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private GameObject target;
    private GameStateSystem stateSystem;
    [SerializeField]
    private NavMeshAgent agent;
    private GameObject currentTarget = null;
    private float attackRange = 1f;
    private float attackDelay = 1f;
    private float lastAttackTime = 0f;
    [SerializeField]
    private CharState charState;
    private GameState currentState;
    private AttackType attackType;
    [SerializeField]
    private GameObject targetObject;
    private HealthSystem healthSystem;
    public Ability ability;
    public Ability superAbility;
    private GameManager gameManager;
    private void Start()
    {
        gameManager = GameManager.Instance;
        unit = GetComponent<Unit>();
        unitObject = unit.GetUnitObject;
        animator = GetComponent<Animator>();
        healthSystem = GetComponent<HealthSystem>();

        stateSystem = GameStateSystem.Instance;
        stateSystem.OnGameStateChanged += GameStateChanged;

        targetObject = GameObject.Find("target");
        attackType = unitObject.attackType;

        currentState = stateSystem.CurrentState;

        if (attackType == AttackType.Melee)
        {
            animator.SetFloat("attackAnim", 0);
        }
        else if (attackType == AttackType.Ranked)
        {
            animator.SetFloat("attackAnim", 1);
        }
    }


    private void Update()
    {

        charState = unit.charState;
        AnimateState(currentState);

    }
    private void GameStateChanged(GameState gameState)
    {
        currentState = GameStateSystem.Instance.CurrentState;
        if (!unit.OnGrid) return;
        if (currentState.IsCombatState)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
            agent.speed = unitObject.speed;
            agent.stoppingDistance = unitObject.attackRange;
        }
        else
        {
            Destroy(agent);
        }
    }
    private void Attack(GameObject target)
    {
        if (ability == null || superAbility == null) return;

        // Calculate the direction to the target
        Vector3 direction = (target.transform.position - transform.position).normalized;

        // Calculate the rotation to face the target
        Quaternion to_Target_Quaternion = Quaternion.LookRotation(direction, Vector3.up);
        IDamageable damagableTarget = target.GetComponent<IDamageable>();

        if (damagableTarget == null) return;
        if (healthSystem.GetMana < healthSystem.GetMaxMana)
        {
            if (attackType == AttackType.Ranked)
            {
                ability.Cast(target, unit.GetUnitObject.attackPower);
            }
            else
            {
                ability.Cast(target, unit.GetUnitObject.attackPower);
            }
            healthSystem.IncreaseMana(healthSystem.GetMaxMana / 4);
        }
        else
        {
            superAbility.Cast(target, unit.GetUnitObject.attackPower);
            healthSystem.DecreaseMana(healthSystem.GetMaxMana);
        }
    }

    private void DefaultMethod()
    {
        animator.SetBool("fall", charState == CharState.Fall);
        animator.SetBool("moving", false);
    }
    private GameObject DetermineTarget()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<Unit> enemyOnGrid = gameManager.GetAllEnemyUnitsOnGrid;
        if (enemies.Length > 0)
        {
            GameObject nearestEnemy = FindNearestEnemy(enemies);
            target = nearestEnemy;
            if (Vector3.Distance(transform.position, nearestEnemy.transform.position) <= attackRange)
            {
                return nearestEnemy;
            }
            else
            {
                return null;
            }
        }
        else if (enemyOnGrid.Count > 0) 
        {
            GameObject nearestEnemy = FindNearestEnemyUnit(enemyOnGrid);
            target = nearestEnemy;
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
    [SerializeField]
    GameObject[] enemies;

    private Vector3 DetermineDestination()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<Unit> enemyOnGrid = gameManager.GetAllEnemyUnitsOnGrid;
        if (enemies.Length > 0)
        {
            GameObject nearestEnemy = FindNearestEnemy(enemies);
            target = nearestEnemy;
            if (Vector3.Distance(transform.position, nearestEnemy.transform.position) <= attackRange)
            {
                return nearestEnemy.transform.position;
            }
            else
            {
                return Vector3.zero;
            }
        }
        else if (enemyOnGrid.Count > 0)
        {
            GameObject nearestEnemy = FindNearestEnemyUnit(enemyOnGrid);
            target = nearestEnemy;
            if (Vector3.Distance(transform.position, nearestEnemy.transform.position) <= attackRange)
            {
                return nearestEnemy.transform.position;
            }
            else
            {
                return Vector3.zero;
            }
        }
        else
        {
            currentTarget = null;
            return Vector3.zero;
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
    private void AnimateState(GameState currentState)
    {
        if (currentState.IsCombatState)
        {
            if (!unit.OnGrid)
            {
                DefaultMethod();
            }
            else
            {
                CombatPhase();
            }
        }
        else {
            DefaultMethod();
        }
    }

    private void CombatPhase()
    {
        if(agent == null) return;
            agent.isStopped = false;
            animator.SetBool("fall", false);
            Vector3 destination = DetermineDestination();
            
            animator.SetBool("moving", agent.velocity.magnitude > 0.3f);
            targetObject.transform.position = destination;
            agent.SetDestination(destination);
            if (enemies.Length == 0) return;

            if (agent.remainingDistance < agent.stoppingDistance && agent.velocity.magnitude < 0.3f)
            {
            float time = Time.time;
                if (time - lastAttackTime >= attackDelay)
                {
                    Attack(target);
                    lastAttackTime = time;
                }
            }
    }
}
