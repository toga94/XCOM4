using System.Collections;
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

        PlayerAI playerAI = gameManager.PlayerAI;
        var players = playerAI.players;
        PlayerData randomPlayer = players[Random.Range(0, players.Count)];

        int stateNum = GameStateSystem.Instance.GetRoundIndex;
       // Debug.LogError("Statenumb " + stateNum);

        List<Vector3> enemyPosition = randomPlayer.roundBoughts[stateNum].gridUnitsPositions;
        List<string> enemyUnitsNames = randomPlayer.roundBoughts[stateNum].gridUnitsName;


        enemyUnits = new List<Unit>();
        for (int i = 0; i < enemyPosition.Count; i++)
        {
            Unit enemyUnit = gameManager.SpawnUnitAtPosition(enemyUnitsNames[i], enemyPosition[i], false);
            Debug.Log(enemyUnitsNames[i] + enemyPosition[i]);
            enemyUnit.OnGrid = true;
            enemyUnit.GetComponent<HealthSystem>().DecreaseMana(999999);
            enemyUnits.Add(enemyUnit);
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
            AfterGame();
            gameManager.LoseCombat(true);
        }
    }

    private void AfterGame()
    {
        try
        {
            unitsOnGrid.ForEach(unit => unit.gameObject.SetActive(true));
        }
        catch (System.Exception)
        {
        }

        DestroyEnemyUnits();
    }

    private void DestroyEnemyUnits()
    {
        float delay = 2f;
        float timer = 0f;

        while (timer < delay)
        {
            timer += Time.deltaTime;
            if (timer >= delay)
            {
                foreach (var unit in enemyUnits)
                {
                    if (unit != null) Destroy(unit.gameObject);
                }
            }
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
            {
                Debug.LogWarning("WinGame");
                gameManager.WinCombat(true);

            }
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
            gameManager.WinCombat(true);
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

        foreach (var floor in floors)
        {
            floor.GetComponent<BoxCollider>().enabled = true;
        }

        IsCombatState = false;
        UnitPositionUtility.RefreshUnitsPosition();
    }
}