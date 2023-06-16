using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class CarouselState : GameState
{
    GameManager gameManager;
    private List<string> availableChampions = new List<string> { "Blightwalker", "Azir", "Qiyana" };

    // Logic for entering Champion Selection state
    public override void OnEnterState()
    {
        duration = 3;
        gameManager = GameManager.Instance;
        string randomChampion = GetRandomChampion();
        gameManager.SpawnUnitAtInventory(randomChampion, true);
    }

    // Logic for updating Champion Selection state
    public override void OnUpdate()
    {

    }

    // Logic for exiting Champion Selection state
    public override void OnExitState()
    {

    }

    private string GetRandomChampion()
    {
        int randomIndex = UnityEngine.Random.Range(0, availableChampions.Count);
        return availableChampions[randomIndex];
    }
}
