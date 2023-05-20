using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Lean.Pool;

public class Minion_1_1_PhaseState : GameState
{
    private List<Enemy> enemies = new List<Enemy>();
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
        foreach (var floor in floors)
        {
            floor.GetComponent<BoxCollider>().enabled = false;
        }

        enemyPool = GameObject.Find("_Pooling").transform.Find("minion_1_1_UIPool").GetComponent<LeanGameObjectPool>();

        List<Vector3> enemyPosition = new List<Vector3>
        {
            new Vector3(-0.5f, 0.24f, 24.5f),
            new Vector3(11f, 0.24f, 20f),
            new Vector3(22f, 0.24f, 24.5f)
        };

        enemies = enemyPosition
            .Select(position => enemyPool.Spawn(position, Quaternion.Euler(0, 180, 0)).GetComponent<Enemy>())
            .ToList();

        enemies.ForEach(enemy =>
        {
            enemy.objectPool = enemyPool;
            enemy.GetComponent<IDamageable>().OnDie += OnEnemyKilled;
        });

        enemiesCount = enemies.Count;

        unitsOnGrid = gameManager.GetAllUnitsOnGrid;
        unitsCount = unitsOnGrid.Count;

        unitsOnGrid.ForEach(unit => unit.GetComponent<HealthSystem>().OnDie += OnUnitKilled);
    }

    private void OnUnitKilled(bool value, GameObject killedUnit)
    {
        unitsCount--;
        killedUnit.SetActive(false);
        allUnitsDead = unitsCount == 0;

        if (allUnitsDead)
        {
            duration = 3f;

            unitsOnGrid.ForEach(unit => unit.gameObject.SetActive(true));
            enemies.ForEach(enemy => enemy.GetComponent<IDamageable>().TakeDamage(99999999));

            gameManager.LoseCombat();
        }
    }

    private void OnEnemyKilled(bool value, GameObject minion)
    {
        enemyPool.Despawn(minion);
        enemiesCount--;

        if (allUnitsDead)
            return;

        allEnemiesDead = enemiesCount == 0;

        if (allEnemiesDead)
            duration = 3f;

        if (unitsCount > 0)
            gameManager.WinCombat();
    }

    public override void OnExitState()
    {
        GameStateSystem.Instance.timeSlider.gameObject.SetActive(true);

        unitsOnGrid.ForEach(unit => unit.gameObject.SetActive(true));

        int winStreak = gameManager.GetWinStreak();
        int bonusGold = winStreak >= 5 ? 3 : winStreak >= 3 ? 2 : 1;

        int totalGold = Economy.MIN_GOLD + winStreak + bonusGold;
        int goldBonus = Mathf.FloorToInt(Economy.GetGold() / 10f);
        totalGold += goldBonus;

        Economy.AddGold(totalGold);
        Economy.GainExperience(1);

        GameObject[] floors = GameObject.FindGameObjectsWithTag("floor");

        foreach (var floor in floors)
        {
            floor.GetComponent<BoxCollider>().enabled = true;
        }

        IsCombatState = false;
        UnitPositionUtility.RefreshUnitsPosition();
    }
}