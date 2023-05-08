using System.Collections.Generic;
using System.Linq;
using TMPro;

public class CarouselState : GameState
{
    GameManager gameManager;
    // Logic for entering Champion Selection state
    public override void OnEnterState()
    {
        duration = 3;
        gameManager = GameManager.Instance;
        gameManager.SpawnUnitAtInventory("Lina");
    }
    // Logic for updating Champion Selection state
    public override void OnUpdate()
    {

    }
    // Logic for exiting Champion Selection state
    public override void OnExitState()
    {
        
    }
}
