using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private PlayerData lastEnemyPlayer;
    private int damageAmount;
    public override void OnEnterState()
    {
        damageAmount = ((GameStateSystem.Instance.GetRoundIndex + 1) * 12) / 2;
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

        List<PlayerData> playersWithHealth = players.Where(player => player.playerHealth > 0).ToList();
        if (playersWithHealth.Count > 0)
        {
            PlayerData randomPlayer = playersWithHealth[Random.Range(0, playersWithHealth.Count)];

            lastEnemyPlayer = randomPlayer;
            PlayerListUI.Instance.CurBattlePlayerAI = randomPlayer;
            int stateNum = GameStateSystem.Instance.GetRoundIndex;

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
        else
        {
            // All players have health less than or equal to 0
            // Load the Victory scene here
            // Example:
            SceneManager.LoadScene(3);
        }
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
            unitsOnGrid.ForEach(unit => unit.gameObject.SetActive(true));

            if (unitsCount > 0)
            {
                Debug.LogWarning("WinGame");
                gameManager.WinCombat(true);

                PlayerAI playerAI = gameManager.PlayerAI;
                foreach (PlayerData player in playerAI.players)
                {
                    if (player.PlayerName == lastEnemyPlayer.PlayerName)
                    {
                        PlayerData modifiedPlayer = player; // Create a separate variable
                      
                        modifiedPlayer.playerHealth -= damageAmount; // damage to modified Player 

                        // Update the player in the playerAI.players list
                        int playerIndex = playerAI.players.IndexOf(player);
                        playerAI.players[playerIndex] = modifiedPlayer;
                    }
                }
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

    private void DamageRandomHalfOfHealthyPlayers()
    {
        PlayerAI playerAI = gameManager.PlayerAI;
        int stateNum = GameStateSystem.Instance.GetRoundIndex;

        List<PlayerData> playersWithHealth = playerAI.players.Where(player => player.playerHealth > 0).ToList();

        // Shuffle the list of players to randomize the order
        System.Random random = new System.Random();
        playersWithHealth = playersWithHealth.OrderBy(player => random.Next()).ToList();

        int numPlayersToDamage = Mathf.CeilToInt(playersWithHealth.Count / 2f) + 1; // Half of the players

        List<PlayerData> playersToModify = new List<PlayerData>();

        for (int i = 0; i < numPlayersToDamage; i++)
        {
            PlayerData player = playersWithHealth[i];
            player.playerHealth -= damageAmount;
            playersToModify.Add(player);
        }

        foreach (var modifiedPlayer in playersToModify)
        {
            foreach (var curPlayer in playerAI.players)
            {
                if (modifiedPlayer.PlayerName == curPlayer.PlayerName && modifiedPlayer.PlayerName != lastEnemyPlayer.PlayerName)
                {
                    int playerIndex = playerAI.players.IndexOf(curPlayer);
                    playerAI.players[playerIndex] = modifiedPlayer;
                    break;
                }
            }
        }
    }


    public override void OnExitState()
    {
        PlayerListUI.Instance.CurBattlePlayerAI = new PlayerData();
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
        DamageRandomHalfOfHealthyPlayers();
        IsCombatState = false;
        UnitPositionUtility.RefreshUnitsPosition();
    }
}