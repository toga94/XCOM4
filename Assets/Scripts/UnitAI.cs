using UnityEngine;
using UnityEngine.AI;

public class UnitAI : MonoBehaviour
{
    private Unit unit;
    private UnitObject unitObject;
    [SerializeField] private Animator animator;
    private GameStateSystem stateSystem;
    [SerializeField] private NavMeshAgent agent;
    private GameObject currentTarget = null;
    private float attackRange = 1f;
    private float attackDelay = 1f;
    private float lastAttackTime = 0f;
    [SerializeField] private CharState charState;
    private GameState currentState;


    [SerializeField] private GameObject targetObject;


    private void Start()
    {
        unit = GetComponent<Unit>();
        unitObject = unit.GetUnitObject;
        animator = GetComponent<Animator>();

        stateSystem = GameStateSystem.Instance;
        stateSystem.OnGameStateChanged += GameStateChanged;

        targetObject = GameObject.Find("target");
    }
    private void Update()
    {
        currentState = GameStateSystem.Instance.GetCurrentState();
        charState = unit.charState;
        AnimateState(currentState);
    }
    private void GameStateChanged(GameState gameState)
    {
        if (!unit.OnGrid) return;
        if (gameState is CombatPhaseState)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
            agent.speed = unitObject.speed;
            agent.stoppingDistance = unitObject.attackType == AttackType.Melee ? 2f : 20f;
        }
        else
        {
            Destroy(agent);
        }
    }
    private void Attack(GameObject target)
    {
        target.GetComponent<IDamageable>().TakeDamage(10);
    }

    private void DefaultMethod()
    {
        animator.SetBool("fall", charState == CharState.Fall);
        animator.SetBool("moving", false);
    }
    private GameObject DetermineTarget()
    {

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
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
    [SerializeField]
    GameObject[] enemies;

    private Vector3 DetermineDestination()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length > 0)
        {
            GameObject nearestEnemy = FindNearestEnemy(enemies);
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
        float closestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestEnemy = enemy;
            }
        }
        return nearestEnemy;
    }
    private void AnimateState(GameState currentState)
    {
        switch (currentState)
        {
            case ChampionSelectionState _:
                DefaultMethod();
                break;
            case CombatPhaseState _:
                {
                    if (!unit.OnGrid)
                    {
                        DefaultMethod();
                    }
                    else
                    {
                        agent.isStopped = false;
                        animator.SetBool("fall", false);
                        Vector3 destination = DetermineDestination();
                        GameObject target = DetermineTarget();
                        animator.SetBool("moving", agent.velocity.magnitude > 0.3f);
                        targetObject.transform.position = destination;
                        agent.SetDestination(destination);
                    }

                    break;
                }
        }
    }
}
