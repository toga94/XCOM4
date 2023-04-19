using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStateSystem : Singleton<GameStateSystem>
{
    public event Action<GameState> OnGameStateChanged;

    private List<GameState> gameStates = new List<GameState>();
    private int currentStateIndex = 0;
    public int GetStateIndex => currentStateIndex;
    public GameStateSystem()
    {
        gameStates.Add(new ChampionSelectionState());
        gameStates.Add(new CombatPhaseState());
    }

    public GameState GetCurrentState()
    {
        return gameStates[currentStateIndex];
    }
    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.P)) {
            ChangeState(0);
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            ChangeState(1);
        }
        gameStates[currentStateIndex].OnUpdate();
    }

    public void ChangeState(int index)
    {
        gameStates[currentStateIndex].OnExitState();
        currentStateIndex = index;
        gameStates[currentStateIndex].OnEnterState();
        OnGameStateChanged?.Invoke(GetCurrentState());
    }
}