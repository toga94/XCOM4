using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Lean.Pool;
public class Minion_1_1_PhaseState : GameState
{
    public List<Enemy> enemies = new List<Enemy>();
    private bool allEnemiesDead = false;
    private bool allUnitsDead = false;
    private GameManager gameManager;
    private int enemiesCount;
    private int unitsCount;
    private List<Unit> unitsOnGrid;

    private LeanGameObjectPool enemyPool;

    public override void OnEnterState()
    {
        gameManager = GameManager.Instance;

        IsCombatState = true;
        duration = 99999f;
        GameStateSystem.Instance.timeSlider.gameObject.SetActive(false);
        UnitPositionUtility.RefreshUnitsPosition();

        GameObject[] floors = GameObject.FindGameObjectsWithTag("floor");
        floors.Select(floor => floor.GetComponent<BoxCollider>()).ToList().ForEach(collider => collider.enabled = false);


        enemyPool = GameObject.Find("_Pooling").transform.Find("minion_1_1_UIPool").GetComponent<LeanGameObjectPool>();

        List<Vector3> enemyPosition = new List<Vector3>
        {
        new Vector3(-0.5f, 0.24f, 24.5f),
        new Vector3(11f, 0.24f, 20f),
        new Vector3(22f, 0.24f, 24.5f)
        };

        List<GameObject> enemyObjects = enemyPosition
        .Select(position => enemyPool.Spawn(position, Quaternion.Euler(0, 180, 0)))
        .ToList();

        enemies.AddRange(enemyObjects.Select(obj => obj.GetComponent<Enemy>()));
        foreach (Enemy enemy  in enemies)
        {
            enemy.objectPool = enemyPool;
        }

        enemiesCount = enemies.Count;
        foreach (IDamageable enemyhp in from enemy in enemies
                                        let enemyhp = enemy.GetComponent<IDamageable>()
                                        select enemyhp)
        {
            enemyhp.OnDie += OnEnemyKilled;
        }
        unitsOnGrid = gameManager.GetAllUnitsOnGrid;
        unitsCount = unitsOnGrid.Count;
        foreach (var unit in unitsOnGrid)
        {
            unit.GetComponent<HealthSystem>().OnDie += OnUnitKilled;
        }


    }

    void OnUnitKilled(bool value, GameObject minion)
    {
        unitsCount--;
        allUnitsDead = unitsCount == 0;
        if (allUnitsDead)
        {
            allUnitsDead = false;
            duration = 3f;

            foreach (var unit in unitsOnGrid)
            {
                unit.gameObject.SetActive(true);
            }
        }
        foreach (var enemy in enemies)
        {
            enemy.GetComponent<IDamageable>().TakeDamage(99999999);
        }
        gameManager.LoseCombat();
    }
    void OnEnemyKilled(bool value, GameObject minion)
    {
        enemyPool.Despawn(minion);
        enemiesCount--;
        allEnemiesDead = enemiesCount == 0;
        if (allEnemiesDead)
        {
            allEnemiesDead = false;
            duration = 3f;
        }
        if(unitsCount > 0)
        gameManager.WinCombat();
    }


    public override void OnExitState()
    {
        GameStateSystem.Instance.timeSlider.gameObject.SetActive(true);

        List<Unit> units = gameManager.GetAllUnitsOnGrid;



        int winStreak = gameManager.GetWinStreak();
        int bonusGold = 0;
        if (winStreak >= 5)
        {
            bonusGold = 3;
        }
        else if (winStreak >= 3)
        {
            bonusGold = 2;
        }
        else
        {
            bonusGold = 1;
        }
        int totalGold = Economy.MIN_GOLD + winStreak + bonusGold;
        int goldBonus = (int)Mathf.FloorToInt(Economy.GetGold() / 10f);
        totalGold += goldBonus;
        Economy.AddGold(totalGold);
        Economy.GainExperience(1);
        GameObject[] floors = GameObject.FindGameObjectsWithTag("floor");
        floors.Select(floor => floor.GetComponent<BoxCollider>()).ToList().ForEach(bc => bc.enabled = true);
        IsCombatState = false;
        UnitPositionUtility.RefreshUnitsPosition();
    }
}