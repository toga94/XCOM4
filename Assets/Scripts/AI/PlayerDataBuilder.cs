public class PlayerDataBuilder
{
    private PlayerData playerData;

    public PlayerDataBuilder()
    {
        playerData = new PlayerData();
    }

    public PlayerDataBuilder SetPlayerName(string playerName)
    {
        playerData.PlayerName = playerName;
        return this;
    }

    public PlayerDataBuilder SetPlayerLevel(int playerLevel)
    {
        playerData.playerLevel = playerLevel;
        return this;
    }

    public PlayerDataBuilder SetPlayerMoney(int playerMoney)
    {
        playerData.playerMoney = playerMoney;
        return this;
    }
    public PlayerDataBuilder SetPlayerHealth(int playerHealth)
    {
        playerData.playerHealth = playerHealth;
        return this;
    }
    public PlayerDataBuilder AddRoundBought(RoundBought roundBought)
    {
        playerData.roundBoughts.Add(roundBought);
        return this;
    }

    public PlayerData Build()
    {
        return playerData;
    }
}
