using System.Collections.Generic;
using System.Linq;
using TMPro;

public class CarouselState : GameState
{
    // Logic for entering Champion Selection state
    public override void OnEnterState()
    {
        GameManager gameManager = GameManager.Instance;
        gameManager.SpawnUnitAtInventory("Lina");
        IsFinished = true;
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
