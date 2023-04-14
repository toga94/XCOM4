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

    [SerializeField] private NavMeshAgent agent;
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

        traits = unitObject.traits;
        dropUnitFX = Resources.Load("DropUnitFX");
    }
    private void Start()
    {
        animator = GetComponent<Animator>();
        stateSystem = GameStateSystem.Instance;
        stateSystem.OnGameStateChanged += GameStateChanged;

        targetObject = GameObject.Find("target");
    }
    public void UpdatePos(TransformData data)
    {
        if (!OnGrid && GameStateSystem.Instance.GetCurrentState() is ChampionSelectionState) return;

        if (agent == null)
        {
            transform.SetPositionAndRotation(data.position, data.rotation);
        }
        else
        {
            agent.Warp(data.position);
        }
    }

    private void GameStateChanged(GameState gameState)
    {
        if (!OnGrid) return;
        if (gameState is CombatPhaseState)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
        }
        else {
            Destroy(agent);
        }
    }
    [SerializeField] private GameObject targetObject;
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

                    if (!OnGrid) { animator.SetBool("fall", charState == CharState.Fall);
                    }
                    else
                    {

                        animator.SetBool("fall", false);
                        Vector3 destination = DetermineDestination();
                        GameObject target = DetermineTarget();

                        targetObject.transform.position = destination;
                        agent.SetDestination(destination);
                        //Debug.Log("fight");

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
        Debug.Log(gameObject.name + " attacks " + target.name);
        target.GetComponent<IDamageable>().TakeDamage(10);
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


}
