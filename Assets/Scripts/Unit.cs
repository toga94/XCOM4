using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    [SerializeField] private string unitName;
    [SerializeField] private int unitLevel;
    public int type; // Type of unit (e.g. archer, knight, mage)
    public int tier; // Tier of unit (e.g. 1-star, 2-star, 3-star)
    public bool OnGrid { get; set; }
    public float MaxHealth { get; set; }
    private Animator animator;
    public string GetUnitNameWithLevel => $"{unitName}{unitLevel}";
    public string GetUnitName => $"{unitName}";
    public int GetUnitLevel => unitLevel;
    public Vector3 UnitPosition;
    public GridPosition UnitGridPosition;
    public List<TraitType> traits;
    public CharState charState;

    [SerializeField]private UnitObject unitObject;
    private Object dropUnitFX;

    public UnitObject GetUnitObject => unitObject;



    private GameStateSystem stateSystem;

    private NavMeshAgent agent;
    private GameObject currentTarget = null;
    private float attackRange = 1f;
    private float attackDelay = 1f;
    private float lastAttackTime = 0f;
    public void UpgradeLevel()
    {
        unitLevel++;
        gameObject.name = GetUnitNameWithLevel;
    }

    public void TeleportToPosition(Vector3 targetPosition, GridPosition unitGridPosition)
    {
        UnitGridPosition = unitGridPosition;
        transform.position = targetPosition;
        UnitPosition = targetPosition;
        Instantiate(dropUnitFX, targetPosition + Vector3.up / 2, Quaternion.identity);

    }
    private void Awake()
    {
        string folderPath = "Data/Units";
        //unitObject = Resources.Load<UnitObject>(folderPath + $"/{GetUnitName}");
        traits = unitObject.traits;
        dropUnitFX = Resources.Load("DropUnitFX");
    }
    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        stateSystem = GameStateSystem.Instance;
        stateSystem.OnGameStateChanged += GameStateChanged;
    }
    private void OnDestroy()
    {
        stateSystem.OnGameStateChanged -= GameStateChanged;
    }
    public void UpdatePos(TransformData data)
    {
        if (!OnGrid) return;

        if (agent == null)
        {
            transform.SetPositionAndRotation(data.position, data.rotation);
        }
        else
        {
            agent.Warp(data.position);
            agent.enabled = false;
            transform.SetPositionAndRotation(data.position, data.rotation);
        }
    }

    private void GameStateChanged(GameState gameState)
    {
        if (!OnGrid) return;
        if (gameState is CombatPhaseState)
        {
            if(agent == null) agent = GetComponent<NavMeshAgent>();
            agent.enabled = true;
            Vector3 destination = DetermineDestination();
            agent.SetDestination(destination);
        }
        else {
            if(!agent.isStopped) agent.isStopped = true;
            agent.enabled = false;
        }
    }

    private void Update()
    {
        GameState currentState = GameStateSystem.Instance.GetCurrentState();

        switch (currentState)
        {
            case ChampionSelectionState _:
                AnimationStates();
                break;
            case CombatPhaseState _:
                {

                    if (!OnGrid) { animator.SetBool("fall", charState == CharState.Fall); return; }
                    animator.SetBool("fall", false);
                    Vector3 destination = DetermineDestination();
                    GameObject target = DetermineTarget();
                    // Set NavMeshAgent destination
                    agent.SetDestination(destination);
                    // Attack target if within attack range
                    if (target != null)
                    {
                        if (Vector3.Distance(transform.position, target.transform.position) <= attackRange)
                        {
                            if (Time.time > lastAttackTime + attackDelay)
                            {
                                Attack(target);
                                lastAttackTime = Time.time;
                            }
                        }
                    }
                    else
                    {
                        agent.isStopped = true;
                    }

                    break;
                }
        }

    }

    private void AnimationStates()
    {
        animator.SetBool("fall", charState == CharState.Fall);
    }

    private void Attack(GameObject target)
    {
        // Perform attack on target
        // Here's a simple example implementation:

        Debug.Log(gameObject.name + " attacks " + target.name);
        target.GetComponent<IDamageable>().TakeDamage(10);
    }

    private GameObject DetermineTarget()
    {
        // Determine target based on current situation
        // This could involve factors like the unit's health, position, etc.
        // Here's a simple example implementation:

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length > 0)
        {
            // If there are enemies, target the nearest one within attack range
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
            // If there are no enemies, clear current target
            currentTarget = null;
            return null;
        }
    }




    private Vector3 DetermineDestination()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length > 0)
        {
            // If there are enemies, move towards the nearest one
            GameObject nearestEnemy = FindNearestEnemy(enemies);
            return nearestEnemy.transform.position;
        }
        else
        {
            // If there are no enemies, move towards a random point within a certain range
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


}
