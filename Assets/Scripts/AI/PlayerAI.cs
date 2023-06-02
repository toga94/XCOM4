using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAI : Singleton<PlayerAI>
{
    public List<PlayerData> players;
    private List<Vector3> enemyGridPositions;
    private List<Vector3> selectedPositions;
    private void Start()
    {
       // GeneratePlayers(7, 9);
    }

    public void GeneratePlayers(int numPlayers, int numRounds)
    {
        players = new List<PlayerData>();

        for (int i = 0; i < numPlayers; i++)
        {
            PlayerData player = new PlayerDataBuilder()
                .SetPlayerName("Player " + (i + 1))
                .SetPlayerLevel(1)
                .SetPlayerMoney(0)
                .Build();

            enemyGridPositions = UnitPositionUtility.EnemyGridPositions();
            selectedPositions = new List<Vector3>();

            for (int j = 0; j < numRounds; j++)
            {
                RoundBought roundBought = GenerateRoundBought(j); // Generate round bought information

                player.roundBoughts.Add(roundBought);
            }

            players.Add(player);
        }
    }

    private RoundBought GenerateRoundBought(int round)
    {
        RoundBought roundBought = new RoundBought();

      //  roundBought.gridUnitsName.Add("Lina");
        roundBought.gridUnitsPositions.Add(GeneratePositions(roundBought.gridUnitsName.Last()));

        
        return roundBought;
    }

    private string GenerateUnits(int round, bool isGrid)
    {
        // Get units for the grid from GameManager
        List<UnitObject> availableUnits = GameManager.Instance.unitObjects.ToList();
        if (round < 5 && !isGrid) return string.Empty;
        // Example: Select random units from the available units
        int randomIndex = Random.Range(0, availableUnits.Count);
        string unit = availableUnits[randomIndex].name;
        return unit;
    }

    private Vector3 GeneratePositions(string unitName)
    {
        Vector3 position = Vector3.zero;


        foreach (UnitObject item in GameManager.Instance.unitObjects)
        {
            if (item.unitName == unitName)
            {
                if (item.attackType == AttackType.Melee)
                {
                    int randomIndex = GetRandomUniqueIndex(0, 13, selectedPositions);
                    position = enemyGridPositions[randomIndex];
                }
                else
                {
                    int randomIndex = GetRandomUniqueIndex(14, 28, selectedPositions);
                    position = enemyGridPositions[randomIndex];
                }

                break;
            }
        }

        return position;
    }

    private int GetRandomUniqueIndex(int minIndex, int maxIndex, List<Vector3> selectedPositions)
    {
        List<int> availableIndexes = Enumerable.Range(minIndex, maxIndex - minIndex + 1).ToList();

        // Remove already selected indexes from the available indexes
        foreach (Vector3 selectedPosition in selectedPositions)
        {
            int selectedIndex = enemyGridPositions.FindIndex(pos => pos == selectedPosition);
            if (selectedIndex >= minIndex && selectedIndex <= maxIndex)
            {
                availableIndexes.Remove(selectedIndex);
            }
        }

        int randomIndex = Random.Range(0, availableIndexes.Count);
        int selectedUniqueIndex = availableIndexes[randomIndex];
        selectedPositions.Add(enemyGridPositions[selectedUniqueIndex]);

        return selectedUniqueIndex;
    }
}