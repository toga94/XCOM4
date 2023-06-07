using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListUI : MonoBehaviour
{
    [SerializeField]
    private Transform playerListContainer;
    [SerializeField]
    private GameObject playerEntryPrefab;
    private LeanGameObjectPool playerUIPool;
    private GameObject childObject;

    void Start()
    {
        GameStateSystem.Instance.OnGameStateChanged += OnGameStateChanged;

        GameObject poolObj = GameObject.Find("_Pooling");
        childObject = new GameObject("PlayerListUIPool");
        childObject.transform.parent = poolObj.transform;
        playerUIPool = childObject.AddComponent<LeanGameObjectPool>();
        playerUIPool.Prefab = playerEntryPrefab;
    }

    private void OnGameStateChanged(GameState obj)
    {
        // Get a reference to the PlayerAI instance
        PlayerAI playerAI = PlayerAI.Instance;

        // Clear the existing player list entries
        ClearPlayerList();
        List<PlayerData> allPlayers = new List<PlayerData>();
        allPlayers = playerAI.players;

        PlayerData player = new PlayerDataBuilder()
                    .SetPlayerName("Player ")
                    .SetPlayerLevel(Economy.Level)
                    .SetPlayerMoney(Economy.GetGold())
                    .SetPlayerHealth(Economy.Health)
                    .Build();

        allPlayers.Add(player);
        // Iterate over each PlayerData object in the players list
        foreach (PlayerData playerData in allPlayers)
        {
            // Instantiate a new player entry prefab
            GameObject playerEntry = playerUIPool.Spawn(playerListContainer);

            // Get the Text components from the player entry prefab
            Text playerNameText = playerEntry.GetComponentInChildren<Text>();

            playerNameText.text = $"{playerData.PlayerName}  {playerData.playerHealth}";
        }
    }

    private void ClearPlayerList()
    {
        playerUIPool.DespawnAll();
    }
}