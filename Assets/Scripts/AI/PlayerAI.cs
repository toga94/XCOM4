using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[System.Serializable]
public class PlayerAI : Singleton<PlayerAI>
{
    [SerializeField]
    public List<PlayerData> players;
    [SerializeField]private List<Vector3> enemyGridPositions;
    private List<Vector3> selectedPositions;
    private void Start()
    {
         GeneratePlayers(7, 9);
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
            player.roundBoughts = new List<RoundBought>();
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

        roundBought.gridUnitsName = new List<string>();
        roundBought.gridUnitsPositions = new List<Vector3>(); // Initialize the list

        roundBought.gridUnitsName.Add(GenerateUnits(round, false));

        if (roundBought.gridUnitsName.Any())
        {
            roundBought.gridUnitsPositions.Add(GeneratePositions(roundBought.gridUnitsName.Last()));
        }

        return roundBought;
    }
    private string GenerateUnits(int round, bool isGrid)
    {
        // Get units for the grid from GameManager
        List<UnitObject> availableUnits = GameManager.Instance.unitObjects.ToList();
        //if (round < 5 && !isGrid) return string.Empty;
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
                    int randomIndex = GetRandomUniqueIndex(1, 13);
                    position = enemyGridPositions[randomIndex];
                }
                else
                {
                    int randomIndex = GetRandomUniqueIndex(14, 27);
                    position = enemyGridPositions[randomIndex];
                }

                break;
            }
        }

        return position;
    }

    private int GetRandomUniqueIndex(int min, int max)
    {
        List<int> availableIndices = new List<int>();

        for (int i = min; i <= max; i++)
        {
            if (!selectedPositions.Contains(enemyGridPositions[i]))
            {
                availableIndices.Add(i);
            }
        }

        if (availableIndices.Count == 0)
        {
            // Handle the case where no unique index is available
            Debug.LogError("No unique index available.");
            return -1; // or any other appropriate value
        }

        int randomIndex = Random.Range(0, availableIndices.Count);
        int selectedIndex = availableIndices[randomIndex];
        selectedPositions.Add(enemyGridPositions[selectedIndex]);

        return selectedIndex;
    }
}