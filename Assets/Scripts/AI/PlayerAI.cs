using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerAI : Singleton<PlayerAI>
{
    [SerializeField]
    public List<PlayerData> players;
    [SerializeField]
    private List<Vector3> enemyGridPositions;
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
                .SetPlayerName(RandomUsernameGenerator.GenerateRandomUsername())
                .SetPlayerLevel(1)
                .SetPlayerMoney(20)
                .SetPlayerHealth(100)
                .Build();
            player.roundBoughts = new List<RoundBought>();
            GenerateEnemyGridPositions();
            selectedPositions = new List<Vector3>();

            for (int j = 0; j < numRounds; j++)
            {
                RoundBought roundBought = GenerateRoundBought(j);
                player.roundBoughts.Add(roundBought);
            }

            players.Add(player);
        }
    }

    private void GenerateEnemyGridPositions()
    {
        enemyGridPositions = UnitPositionUtility.EnemyGridPositions().ToList();
    }

    private RoundBought GenerateRoundBought(int round)
    {
        RoundBought roundBought = new RoundBought();

        roundBought.gridUnitsName = new List<string>();
        roundBought.gridUnitsPositions = new List<Vector3>();

        int numUnits = GetNumUnits(round);

        for (int i = 0; i < numUnits; i++)
        {
            GenerateUnitWithPosition(roundBought, round);
        }

        return roundBought;
    }

    private int GetNumUnits(int round)
    {
        if (round < 2)
        {
            return 2;
        }
        else if (round <= 3)
        {
            return 3;
        }
        else if (round <= 4)
        {
            return 4;
        }
        else if (round <= 5)
        {
            return 5;
        }
        else
        {
            return round * 2;
        }
    }

    private void GenerateUnitWithPosition(RoundBought roundBought, int round)
    {
        string unitName = GenerateUnits(round, false);
        Vector3 unitPosition = GeneratePositions(unitName);

        if (unitPosition != Vector3.zero)
        {
            roundBought.gridUnitsName.Add(unitName);
            roundBought.gridUnitsPositions.Add(unitPosition);
        }
    }

    private string GenerateUnits(int round, bool isGrid)
    {
        GameManager gameManager = GameManager.Instance;
        if (gameManager == null) return string.Empty;
        List<UnitObject> availableUnits = gameManager.unitObjects.ToList();
        int randomIndex = Random.Range(0, availableUnits.Count);
        string unit = availableUnits[randomIndex].name;
        return unit;
    }

    private Vector3 GeneratePositions(string unitName)
    {
        Vector3 position = Vector3.zero;

        foreach (UnitObject item in GameManager.Instance.unitObjects)
        {
            if (item.unitName.Contains(unitName))
            {
                List<int> availableIndices = GetAvailableIndices(item.attackType == AttackType.Melee ? 1 : 14, item.attackType == AttackType.Melee ? 13 : 27);

                if (availableIndices.Count == 0)
                {
                    Debug.LogWarning("No unique index available for unit: " + unitName);
                    return position;
                }

                int randomIndex = Random.Range(0, availableIndices.Count);
                int selectedIndex = availableIndices[randomIndex];
                position = enemyGridPositions[selectedIndex];
                selectedPositions.Add(position);

                break;
            }
        }

        return position;
    }

    private List<int> GetAvailableIndices(int min, int max)
    {
        List<int> availableIndices = new List<int>();

        for (int i = min; i <= max; i++)
        {
            if (!selectedPositions.Contains(enemyGridPositions[i]))
            {
                availableIndices.Add(i);
            }
        }

        return availableIndices;
    }
}
