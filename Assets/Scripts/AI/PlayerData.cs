using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PlayerData
{
    public string PlayerName;
    public int playerLevel;
    public int playerMoney;
    public int playerHealth;
    [SerializeField]
    public List<RoundBought> roundBoughts;
}
