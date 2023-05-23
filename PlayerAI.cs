using System.Collections.Generic;
using UnityEngine;

public class PlayerAI : MonoBehaviour
{
    public List<PlayerData> players;

    private void Start()
    {
        GeneratePlayers(7, 9);
    }

    private void GeneratePlayers(int numPlayers, int numRounds)
    {
        players = new List<PlayerData>();

        for (int i = 0; i < numPlayers; i++)
        {
            PlayerData player = new PlayerDataBuilder()
                .SetPlayerName("Player " + (i + 1))
                .SetPlayerLevel(1)
                .SetPlayerMoney(0)
                .Build();

            for (int j = 0; j < numRounds; j++)
            {
                RoundBought roundBought = new RoundBoughtBuilder()
                    // Add bought units for the round
                    // Modify this part based on your logic
                    .Build();

                player.roundBoughts.Add(roundBought);
            }

            players.Add(player);
        }
    }
}