using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameStateSystem : Singleton<GameStateSystem>
{
    public event Action<GameState> OnGameStateChanged;

    private List<GameState> gameStates = new List<GameState>();
    private int currentStateIndex = 0;
    private Coroutine stateCoroutine;

    public int GetStateIndex => currentStateIndex;
    public GameStateSystem()
    {
        // Create the list of game states
        gameStates.AddRange(new GameState[] {
        new CarouselState(),
        new ChampionSelectionState(),
        new CombatPhaseState(),
        new ChampionSelectionState(),
        new CombatPhaseState(),
        new ChampionSelectionState(),
        new CombatPhaseState(),
        new ChampionSelectionState(),
        new CombatPhaseState(),
        new ChampionSelectionState(),
        new CombatPhaseState(),
        new CarouselState(),
        new ChampionSelectionState(),
        new CombatPhaseState(),
        new ChampionSelectionState(),
        new CombatPhaseState(),
        new ChampionSelectionState(),
        new CombatPhaseState(),
        new ChampionSelectionState(),
        new CombatPhaseState(),
        new ChampionSelectionState(),
        new CombatPhaseState(),
        new ChampionSelectionState(),
        new CombatPhaseState(),
        new ChampionSelectionState(),
        new CombatPhaseState(),
        new ChampionSelectionState(),
        new CombatPhaseState(),
        new CarouselState(),
        new ChampionSelectionState(),
        new CombatPhaseState(),
        new ChampionSelectionState(),
        new CombatPhaseState(),
        new ChampionSelectionState(),
        new CombatPhaseState(),
        new ChampionSelectionState(),
        new CombatPhaseState(),
        new ChampionSelectionState(),
        new CombatPhaseState(),
        new ChampionSelectionState(),
        new CombatPhaseState(),
        new CarouselState(),
        new ChampionSelectionState(),
        new CombatPhaseState(),
        new ChampionSelectionState(),
        new CombatPhaseState(),
        new ChampionSelectionState(),
        new CombatPhaseState(),
        new ChampionSelectionState(),
        new CombatPhaseState(),
        new ChampionSelectionState(),
        new CombatPhaseState(),
        new ChampionSelectionState()
    });
    }

    private void Start()
    {
        stateCoroutine = StartCoroutine(GameLoop());
    }
    public GameState GetCurrentState()
    {
        return gameStates.ElementAtOrDefault(currentStateIndex);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            GetCurrentState().IsFinished = true;
        }

        gameStates[currentStateIndex].OnUpdate();
    }
    private IEnumerator GameLoop()
    {
        // Start the first state
        gameStates[currentStateIndex].OnEnterState();

        // Loop until the game is over
        while (true)
        {
            // Wait for the current state to finish
            yield return new WaitUntil(() => GetCurrentState().IsFinished);

            // Move to the next state
            ChangeState((currentStateIndex + 1) % gameStates.Count);
        }
    }

    public void ChangeState(int index)
    {
        gameStates[currentStateIndex].OnExitState();

        if (index < 0 || index >= gameStates.Count)
        {
            index = gameStates.Count - 1;
        }

        currentStateIndex = index;
        gameStates[currentStateIndex].OnEnterState();
        OnGameStateChanged?.Invoke(GetCurrentState());
    }
}