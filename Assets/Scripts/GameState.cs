using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameState : MonoBehaviour
{
    protected GameStateSystem gameStateSystem;

    public bool IsFinished { get; set; }

    public void SetGameStateSystem(GameStateSystem gameStateSystem)
    {
        this.gameStateSystem = gameStateSystem;
    }

    public virtual void OnEnterState() { }
    public virtual void OnUpdate() { }
    public virtual void OnExitState() { }
}
