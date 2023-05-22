using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCombat_PhaseState : GameState
{
    private List<Unit> enemyUnits = new List<Unit>();
    private bool allEnemiesDead = false;
    private bool allUnitsDead = false;
    private GameManager gameManager;
    private int enemiesCount;
    private int unitsCount;
    private int enemyUnitsCount;
    private List<Unit> unitsOnGrid;
    private List<Unit> enemyUnitsOnGrid;
    private GameObject[] floors;

    public override void OnEnterState()
    {
        gameManager = GameManager.Instance;

        IsCombatState = true;
        duration = 99999f;
        GameStateSystem.Instance.timeSlider.gameObject.SetActive(false);
        UnitPositionUtility.RefreshUnitsPosition();

         floors = GameObject.FindGameObjectsWithTag("floor");
        foreach (var floor in floors)
        {
            floor.GetComponent<BoxCollider>().enabled = false;
        }

    
        List<Vector3> enemyPosition = new List<Vector3>
        {
            new Vector3(-0.5f, 0.24f, 24.5f),
            new Vector3(11f, 0.24f, 20f),
            new Vector3(22f, 0.24f, 24.5f)
        };

        enemyUnits = enemyPosition
            .Select(position => gameManager.SpawnUnitAtPosition("Lina", position, false))
            .ToList();

        foreach (var enemyUnit in enemyUnits)
        {
            enemyUnit.OnGrid = true;
            enemyUnit.GetComponent<HealthSystem>().DecreaseMana(999999);
        }
        enemiesCount = enemyUnits.Count;

        unitsOnGrid = gameManager.GetAllUnitsOnGrid;
        enemyUnitsOnGrid = gameManager.GetAllEnemyUnitsOnGrid;
        unitsCount = unitsOnGrid.Count;
        enemyUnitsCount = enemyUnitsOnGrid.Count;
        enemyUnitsOnGrid.ForEach(unit => unit.GetComponent<HealthSystem>().OnDie += OnEnemyUnitKilled);
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

            gameManager.LoseCombat();
        }
    }
    private void OnEnemyUnitKilled(bool value, GameObject killedUnit)
    {
        enemyUnitsCount--;
        killedUnit.SetActive(false);
        Destroy(killedUnit);
        allEnemiesDead = enemyUnitsCount == 0;

        if (allEnemiesDead)
        {
            duration = 3f;
            Debug.LogWarning("allenemyisdead");
            unitsOnGrid.ForEach(unit => unit.gameObject.SetActive(true));
          
            if (unitsCount > 0)
                gameManager.WinCombat();
        }
    }
    private void OnEnemyKilled(bool value, GameObject minion)
    {

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
        foreach (var unit in enemyUnits)
        {
            Destroy(unit);
        }
        foreach (var floor in floors)
        {
            floor.GetComponent<BoxCollider>().enabled = true;
        }

        IsCombatState = false;
        UnitPositionUtility.RefreshUnitsPosition();
    }
}