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
    private AttackType attackType;
    [SerializeField] private GameObject targetObject;
    private HealthSystem healthSystem;
    public Ability ability;
    public Ability superAbility;

    private void Start()
    {
        unit = GetComponent<Unit>();
        unitObject = unit.GetUnitObject;
        animator = GetComponent<Animator>();
        healthSystem = GetComponent<HealthSystem>();
        stateSystem = GameStateSystem.Instance;
        stateSystem.OnGameStateChanged += GameStateChanged;

        targetObject = GameObject.Find("target");
        attackType = unitObject.attackType;

        currentState = GameStateSystem.Instance.GetCurrentState();

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
        currentState = GameStateSystem.Instance.GetCurrentState();
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
        if (ability == null || superAbility == null) return;

        // Calculate the direction to the target
        Vector3 direction = (target.transform.position - transform.position).normalized;

        // Calculate the rotation to face the target
        Quaternion to_Target_Quaternion = Quaternion.LookRotation(direction, Vector3.up);
        IDamageable damagableTarget = target.GetComponent<IDamageable>();

        if (damagableTarget == null) return;
        if (healthSystem.GetMana < superAbility.ManaCost)
        {
            if (attackType == AttackType.Ranked)
            {
                ability.Cast(target);
                damagableTarget.TakeDamage(ability.AbilityPower + unit.GetUnitObject.attackPower);
            }
            else
            {
                ability.Cast(target);
                damagableTarget.TakeDamage(ability.AbilityPower + unit.GetUnitObject.attackPower);
            }
            Debug.Log("Abiliy");
            healthSystem.IncreaseMana(1);
        }
        else
        {
            superAbility.Cast(target);
            healthSystem.DecreaseMana(superAbility.ManaCost);
            damagableTarget.TakeDamage(superAbility.AbilityPower + unit.GetUnitObject.attackPower);
            Debug.Log("SuperAbiliy");
        }
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
                        CombatPhase();
                    }

                    break;
                }
        }
    }
    [SerializeField] private GameObject target;
    private void CombatPhase()
    {
        agent.isStopped = false;
        animator.SetBool("fall", false);
        Vector3 destination = DetermineDestination();

        animator.SetBool("moving", agent.velocity.magnitude > 0.3f);
        targetObject.transform.position = destination;
        agent.SetDestination(destination);

        if (enemies.Length == 0) return;

        if (agent.remainingDistance < agent.stoppingDistance  && agent.velocity.magnitude < 0.3f)
        {
            if (Time.time - lastAttackTime >= attackDelay)
            {
                Attack(target);
                lastAttackTime = Time.time;
            }
        }
    }
}
