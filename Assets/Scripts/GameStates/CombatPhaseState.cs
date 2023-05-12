using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatPhaseState : GameState
{
    private List<TransformData> unitTransforms = new List<TransformData>();
    public List<Enemy> enemies = new List<Enemy>();
    private bool allEnemiesDead = false;

    private int enemiesCount;
    public override void OnEnterState()
    {
        IsCombatState = true;
        duration = 99999f;
        GameStateSystem.Instance.timeSlider.gameObject.SetActive(false);
        GameManager gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogError("GameManager instance is null!");
            return;
        }
        unitTransforms = gm.GetAllUnitsOnGrid.Select(unit => new TransformData(unit.transform.position, unit.transform.rotation)).ToList();
        gm.SavedUnitTransforms = unitTransforms;
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
    void OnEnemyKilled(bool value)
    {
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
        GameManager gm = GameManager.Instance;
        List<Unit> units = gm.GetAllUnitsOnGrid;
        List<TransformData> savedTransforms = gm.SavedUnitTransforms;
        units.Select((unit, index) => new { unit, index }).ToList().ForEach(obj => {
            obj.unit.transform.SetPositionAndRotation(
                savedTransforms[obj.index].position, savedTransforms[obj.index].rotation);
        });

        gm.WinCombat();
        int winStreak = gm.GetWinStreak();
        int bonusGold = 0;
        if (winStreak >= 5)
        {
            bonusGold = 3;
        }
        else if (winStreak >= 3)
        {
            bonusGold = 2;
        }
        else if (winStreak >= 2)
        {
            bonusGold = 1;
        }
        int totalGold = Economy.MIN_GOLD + winStreak + bonusGold;
        int goldBonus = (int)Mathf.FloorToInt(Economy.GetGold() / 10f);
        totalGold += goldBonus;
        Economy.AddGold(totalGold);
        GameObject[] floors = GameObject.FindGameObjectsWithTag("floor");
        floors.Select(floor => floor.GetComponent<BoxCollider>()).ToList().ForEach(bc => bc.enabled = true);
        IsCombatState = false;
    }
}