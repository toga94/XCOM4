using Lean.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListUI : Singleton<PlayerListUI>
{
    [SerializeField]
    private Transform playerListContainer;
    [SerializeField]
    private GameObject playerEntryPrefab;
    [SerializeField]
    private LeanGameObjectPool playerUIPool;
    private GameObject childObject;
    private PlayerAI playerAI;

    public PlayerData CurBattlePlayerAI;
    void Start()
    {
        GameStateSystem.Instance.OnGameStateChanged += OnGameStateChanged;
        playerAI = GetComponent<PlayerAI>();
        GameObject poolObj = GameObject.Find("_Pooling");
        childObject = new GameObject("PlayerListUIPool");
        childObject.transform.parent = poolObj.transform;
        playerUIPool = childObject.AddComponent<LeanGameObjectPool>();
        playerUIPool.Prefab = playerEntryPrefab;

    }
    List<PlayerData> allPlayers;
    Material playerEntryMaterialInstance;
    private void OnGameStateChanged(GameState obj)
    {
        ClearPlayerList();

        allPlayers = new List<PlayerData>(playerAI.players);

        allPlayers.Add(new PlayerData
        {
            PlayerName = "Me",
            playerHealth = Economy.Health,
            playerLevel = Economy.Level,
            playerMoney = Economy.GetGold()
        });

        foreach (PlayerData playerData in allPlayers)
        {
            GameObject playerEntry = playerUIPool.Spawn(playerListContainer);
            Text playerNameText = playerEntry.GetComponentInChildren<Text>();

            playerEntryMaterialInstance = Instantiate( 
                playerEntry.GetComponent<Image>().material);
            if (CurBattlePlayerAI.PlayerName == playerData.PlayerName)
            {
                playerEntryMaterialInstance.EnableKeyword("OUTBASE_ON");
                playerEntryMaterialInstance.EnableKeyword("WAVEUV_ON");
            }
            else {
                playerEntryMaterialInstance.DisableKeyword("OUTBASE_ON");
                playerEntryMaterialInstance.DisableKeyword("WAVEUV_ON");
            }
            if (playerData.PlayerName.Equals("Me"))
            {
                playerEntryMaterialInstance.EnableKeyword("CHANGECOLOR_ON");
            }
            else {
                playerEntryMaterialInstance.DisableKeyword("CHANGECOLOR_ON");
            }
            
            playerEntry.GetComponent<Image>().material = playerEntryMaterialInstance;
            if (playerData.playerHealth > 0)
            {
                playerNameText.text = $"{playerData.PlayerName}  {playerData.playerHealth}";
                if (CurBattlePlayerAI.PlayerName != playerData.PlayerName)
                {
                    playerNameText.color = Color.white;
                }
                else {
                    playerNameText.color = Color.yellow;
                }
            }
            else
            {
                playerNameText.text = StrikeThrough($"{playerData.PlayerName}  {playerData.playerHealth}");
                playerNameText.color = Color.grey;
            }
        }
    }
    public string StrikeThrough(string s)
    {
        string strikethrough = "";
        foreach (char c in s)
        {
            strikethrough = strikethrough + c + '\u0336';
        }
        return strikethrough;
    }
    private void ClearPlayerList()
    {
        if (playerEntryMaterialInstance != null) Destroy(playerEntryMaterialInstance);
        if (playerUIPool != null)
        {
            playerUIPool.DespawnAll();
        }

        if (allPlayers != null && allPlayers.Count > 0)
        {
            allPlayers.Clear();
        }
    }
}
