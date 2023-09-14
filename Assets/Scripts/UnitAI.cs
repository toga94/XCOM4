using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using MoreMountains.Feedbacks;
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
    [SerializeField]
    private List<Unit> enemyOnGrid;
    [SerializeField]
    private MMF_Player mmf_player;
    private GameObject nearestEnemy;
    private void Start()
    {
        gameManager = GameManager.Instance;
        unit = GetComponent<Unit>();
        unitObject = unit.GetUnitObject;
        animator = GetComponent<Animator>();
        healthSystem = GetComponent<HealthSystem>();
        stateSystem = GameStateSystem.Instance;
        currentState = stateSystem.GetCurrentState;
        stateSystem.OnGameStateChanged += GameStateChanged;
        if (!unit.isOwn)
        {
            GameStateChanged(currentState);
        }
     //   mmf_player = gameManager.GetMMF_Player;



        targetObject = GameObject.Find("target");
        attackType = unitObject.attackType;

        healthSystem.DecreaseMana(healthSystem.GetMaxMana);

    }


    private void Update()
    {
        currentState = stateSystem.GetCurrentState;
        if (currentState is ChampionSelectionState && !unit.isOwn)
        {
            Destroy(gameObject);
        }
        charState = unit.charState;
        AnimateState(currentState);

    }
    private void GameStateChanged(GameState gameState)
    {
        currentState = GameStateSystem.Instance.GetCurrentState;
        if (unit == null || !unit.OnGrid)
            return;

        if (currentState.IsCombatState)
        {
            if (agent == null)
            {
                agent = gameObject.AddComponent<NavMeshAgent>();
                agent.speed = unitObject.speed;
                agent.stoppingDistance = unitObject.attackRange;
            }
        }
        else
        {
            if (unit.isOwn)
            {
                transform.rotation = Quaternion.identity;
            }

            if (agent != null)
            {
                Destroy(agent);
                agent = null;
            }
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
        if (unit.isOwn)
        {
            enemies = GameObject.FindGameObjectsWithTag("Enemy");

            enemyOnGrid = gameManager.GetAllEnemyUnitsOnGrid;
        }
        else
        {
            enemyOnGrid = gameManager.GetAllUnitsOnGrid;
        }

        if (enemies.Length > 0)
        {
            nearestEnemy = FindNearestEnemy(enemies);
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
            nearestEnemy = FindNearestEnemyUnit(enemyOnGrid);
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

        if (unit.isOwn)
        {
            enemies = GameObject.FindGameObjectsWithTag("Enemy");

            enemyOnGrid = gameManager.GetAllEnemyUnitsOnGrid;
        }
        else
        {
            enemyOnGrid = gameManager.GetAllUnitsOnGrid;
        }
        if (enemies.Length > 0)
        {
            nearestEnemy = FindNearestEnemy(enemies);
            target = nearestEnemy;
            return nearestEnemy.transform.position;
        }
        else if (enemyOnGrid.Count > 0)
        {
            nearestEnemy = FindNearestEnemyUnit(enemyOnGrid);
            target = nearestEnemy;
            return nearestEnemy.transform.position;
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
        else
        {
            DefaultMethod();
        }
    }

    private void CombatPhase()
    {
        if (agent == null) return;
        agent.isStopped = false;
        animator.SetBool("fall", false);
        Vector3 destination = DetermineDestination();
        animator.SetBool("moving", agent.velocity.magnitude > 0.3f);
        //targetObject.transform.position = destination;
        agent.SetDestination(destination);
        if (enemies.Length == 0 && enemyOnGrid.Count == 0) return;

        if (agent.remainingDistance < agent.stoppingDistance && agent.velocity.magnitude < 0.3f)
        {
            transform.LookAt(new Vector3(nearestEnemy.transform.position.x, transform.position.y, nearestEnemy.transform.position.z));
            if (attackType == AttackType.Melee)
            {
                animator.SetFloat("attackAnim", 0);
            }
            else if (attackType == AttackType.Ranked)
            {
                animator.SetFloat("attackAnim", 1);
            }
            float time = Time.time;
            if (time - lastAttackTime >= attackDelay)
            {
                if (agent.pathStatus == NavMeshPathStatus.PathComplete)
                {
                    agent.isStopped = true;
                    try
                    {
                        Attack(target);
                    }
                    catch (System.Exception)
                    {

                    }
                    lastAttackTime = time;
                }
            }
        }
    }
}