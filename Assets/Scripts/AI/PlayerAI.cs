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
        InitializePlayers(7, 9);
    }

    public void InitializePlayers(int numPlayers, int numRounds)
    {
        players = new List<PlayerData>();

        for (int i = 0; i < numPlayers; i++)
        {
            PlayerData player = new PlayerDataBuilder()
                .SetPlayerName(RandomUsernameGenerator.GenerateRandomUsername())
                .SetPlayerLevel(1)
                .SetPlayerMoney(40)
                .SetPlayerHealth(100)
                .Build();
            player.roundBoughts = new List<RoundBought>();
            SetupEnemyGridPositions();
            selectedPositions = new List<Vector3>();

            for (int j = 0; j < numRounds; j++)
            {
                RoundBought roundBought = CreateRoundBoughtDetails(j);
                player.roundBoughts.Add(roundBought);
            }

            players.Add(player);
        }
    }

    private void SetupEnemyGridPositions()
    {
        enemyGridPositions = UnitPositionUtility.EnemyGridPositions().ToList();
    }

    private RoundBought CreateRoundBoughtDetails(int round)
    {
        RoundBought roundBought = new RoundBought();

        roundBought.gridUnitsName = new List<string>();
        roundBought.gridUnitsPositions = new List<Vector3>();

        int numUnits = CalculateUnitsPerRound(round);

        for (int i = 0; i < numUnits; i++)
        {
            AssignUnitToPosition(roundBought, round);
        }

        return roundBought;
    }

    private int CalculateUnitsPerRound(int round)
    {
        if (round < 2)
        {
            return 2;
        }
        else if (round <= 5)
        {
            return round + 1;
        }
        else
        {
            return round * 2;
        }
    }

    private void AssignUnitToPosition(RoundBought roundBought, int round)
    {
        string unitName = SelectRandomUnit(round, false);
        Vector3 unitPosition = DetermineUnitPosition(unitName);

        if (unitPosition != Vector3.zero)
        {
            roundBought.gridUnitsName.Add(unitName);
            roundBought.gridUnitsPositions.Add(unitPosition);
        }
    }

    private string SelectRandomUnit(int round, bool isGrid)
    {
        GameManager gameManager = GameManager.Instance;
        if (gameManager == null) return string.Empty;
        List<UnitObject> availableUnits = gameManager.unitObjects.ToList();
        int randomIndex = Random.Range(0, availableUnits.Count);
        string unit = availableUnits[randomIndex].name;
        return unit;
    }

    private Vector3 DetermineUnitPosition(string unitName)
    {
        Vector3 position = Vector3.zero;
        var unitObject = GameManager.Instance.unitObjects;
        foreach (UnitObject item in unitObject)
        {
            if (item.unitName.Contains(unitName))
            {
                List<int> availableIndices = FindOpenPositions(item.attackType == AttackType.Melee ? 1 : 14, item.attackType == AttackType.Melee ? 13 : 27);

                if (availableIndices.Count == 0)
                {
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

    private List<int> FindOpenPositions(int min, int max)
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
