using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class GameStateSystem : Singleton<GameStateSystem>
{
    public event Action<GameState> OnGameStateChanged;

    private List<GameState> gameStates = new List<GameState>();
    private int currentStateIndex = 0;
    private Coroutine stateCoroutine;


    public Slider timeSlider;

    // Maximum durations for each state (in seconds)
    private float carouselDuration = 30f;
    private float minionsDuration = 30f;
    private float randomUserDuration = 15f;
    private float krugsDuration = 30f;
    private float murkWolvesDuration = 30f;
    private float aurelionDoomDuration = 30f;
    private float nemesisMorganaDuration = 30f;
    private float giantCrabgotDuration = 30f;

    // Time when the current state started
    private float currentStateStartTime;

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
        new CombatPhaseState()
        });
    }

    public GameState GetCurrentState()
    {
        return gameStates.ElementAtOrDefault(currentStateIndex);
    }
    private void Start()
    {
        gameStates[currentStateIndex].OnEnterState();
    }

    public void Update()
    {
        GameState currentState = GetCurrentState();
        if (Input.GetKeyDown(KeyCode.P))
        {
            currentState.IsFinished = true;
        }
        if (currentState.IsFinished)
        {
            ChangeState((currentStateIndex + 1) % gameStates.Count);
        }
        gameStates[currentStateIndex].OnUpdate();

        float timer = Time.time - currentStateStartTime;
        if (timer > currentState.duration)
        {
            currentState.IsFinished = true;
            
        }
        timeSlider.value = timer;
        timeSlider.maxValue = currentState.duration;

    }


    public void ChangeState(int index)
    {
        GameState currentState = GetCurrentState();

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