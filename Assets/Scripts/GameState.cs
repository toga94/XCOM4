using UnityEngine;
[System.Serializable]
public class GameState : MonoBehaviour
{
    protected GameStateSystem gameStateSystem;
    public float duration;
    public bool IsFinished { get; set; }
    public bool IsWin { get; set; }
    private bool isCombatState;

    public bool IsCombatState
    {
        get { return isCombatState; }
        protected set { isCombatState = value; }
    }

    public void SetGameStateSystem(GameStateSystem gameStateSystem)
    {
        this.gameStateSystem = gameStateSystem;
    }

    public virtual void OnEnterState() { }
    public virtual void OnUpdate() { }
    public virtual void OnExitState() { }
}
