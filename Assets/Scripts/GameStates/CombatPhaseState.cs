using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatPhaseState : GameState
{
    private List<TransformData> unitTransforms = new List<TransformData>();
    public List<Enemy> enemies = new List<Enemy>();
    private int enemyCount;
    
    // Logic for entering Combat Phase state
    public override void OnEnterState()
    {
        duration = 99999f;
        GameStateSystem.Instance.timeSlider.gameObject.SetActive(false);
        GameManager gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogError("GameManager instance is null!");
            return;
        }

        // Save all unit transforms
        unitTransforms = gm.GetAllUnitsOnGrid.Select(unit => new TransformData(unit.transform.position, unit.transform.rotation)).ToList();
        gm.SavedUnitTransforms = unitTransforms;

        // Disable all box colliders with tag "floor"
        GameObject[] floors = GameObject.FindGameObjectsWithTag("floor");
        floors.Select(floor => floor.GetComponent<BoxCollider>()).ToList().ForEach(collider => collider.enabled = false);


        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        enemies.AddRange(enemyObjects.Select(obj => obj.GetComponent<Enemy>()));

    }
    private void HandleEnemyDie(bool isDead)
    {
        enemyCount--;
        Debug.Log("Enemy Died !" + enemyCount);
        GameState curState = GameStateSystem.Instance.GetCurrentState();
        if (curState is CombatPhaseState)
        {
            if (enemyCount <= 0) GameStateSystem.Instance.GetCurrentState().IsFinished = true;
        }
    }

    // Logic for updating Combat Phase state
    public override void OnUpdate()
    {
        // TODO: Add combat logic

        bool allEnemiesDead = true;
        foreach (Enemy enemy in enemies)
        {
            if (!enemy.isDead)
            {
                allEnemiesDead = false;
                break;
            }
        }

        if (allEnemiesDead)
        {
            enemies.Clear();
            IsFinished = true;
        }

    }

    // Logic for exiting Combat Phase state
    public override void OnExitState()
    {
        GameStateSystem.Instance.timeSlider.gameObject.SetActive(true);
        GameManager gm = GameManager.Instance;
        List<Unit> units = gm.GetAllUnitsOnGrid;
        List<TransformData> savedTransforms = gm.SavedUnitTransforms;
        // Move units back to saved positions and rotations
        units.Select((unit, index) => new { unit, index }).ToList().ForEach(obj => {
            obj.unit.transform.SetPositionAndRotation(savedTransforms[obj.index].position, savedTransforms[obj.index].rotation);
        });

        gm.WinCombat();



        // Calculate the player's current win streak
        int winStreak = gm.GetWinStreak();

        // Calculate the amount of bonus gold based on the player's win streak
        int bonusGold = 0;
        if (winStreak >= 2)
        {
            bonusGold = 1;
        }
        if (winStreak >= 3)
        {
            bonusGold = 2;
        }
        if (winStreak >= 5)
        {
            bonusGold = 3;
        }

        // Calculate the total gold that the player will receive
        int totalGold = Economy.MIN_GOLD + winStreak + bonusGold;

        // Calculate the bonus gold for every 10 gold
        int goldBonus = (int)Mathf.FloorToInt(Economy.GetGold() / 10f);

        // Add the bonus gold for every 10 gold to the total gold
        totalGold += goldBonus;


        Economy.AddGold(totalGold);
        GameObject[] floors = GameObject.FindGameObjectsWithTag("floor");
        floors.Select(floor => floor.GetComponent<BoxCollider>()).ToList().ForEach(bc => bc.enabled = true);


    }
}