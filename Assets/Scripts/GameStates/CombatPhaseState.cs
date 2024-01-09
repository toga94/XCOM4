using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatPhaseState : GameState
{
    public List<Enemy> enemies = new List<Enemy>();
    private bool allEnemiesDead = false;
    private GameManager gameManager;
    private int enemiesCount;
    public override void OnEnterState()
    {
        gameManager = GameManager.Instance;

        IsCombatState = true;
        duration = 99999f;
        GameStateSystem.Instance.timeSlider.gameObject.SetActive(false);
        UnitPositionUtility.RefreshUnitsPosition();

        GameObject[] floors = GameObject.FindGameObjectsWithTag("floor");
        floors.Select(floor => floor.GetComponent<BoxCollider>()).ToList().ForEach(collider => collider.enabled = false);

        List<GameObject> enemyObjects = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        enemies.AddRange(enemyObjects.Select(obj => obj.GetComponent<Enemy>()));
        enemiesCount = enemies.Count;
        foreach (IDamageable enemyhp in from enemy in enemies
                                        let enemyhp = enemy.GetComponent<IDamageable>()
                                        select enemyhp)
        {
            enemyhp.OnDie += OnEnemyKilled;
        }
    }



    void OnEnemyKilled(bool value, GameObject minion)
    {
        //enemyPool.Despawn(minion);
        enemiesCount--;
        allEnemiesDead = enemiesCount == 0;
        if (allEnemiesDead)
        {
            allEnemiesDead = false;
            duration = 3f;
        }
    }



    public override void OnExitState()
    {
        GameStateSystem.Instance.timeSlider.gameObject.SetActive(true);

        List<Unit> units = gameManager.GetAllUnitsOnGrid;


        gameManager.WinCombat(false);
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
        int totalGold = EconomyManager.MIN_GOLD + winStreak + bonusGold;
        int goldBonus = (int)Mathf.FloorToInt(EconomyManager.GetGold() / 10f);
        totalGold += goldBonus;
        EconomyManager.AddGold(totalGold);
        EconomyManager.GainExperience(1);
        GameObject[] floors = GameObject.FindGameObjectsWithTag("floor");
        floors.Select(floor => floor.GetComponent<BoxCollider>()).ToList().ForEach(bc => bc.enabled = true);
        IsCombatState = false;
        UnitPositionUtility.RefreshUnitsPosition();
    }
}