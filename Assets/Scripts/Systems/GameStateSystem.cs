using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class GameStateSystem : Singleton<GameStateSystem>
{
    public event Action<GameState> OnGameStateChanged;
    public float currentDuration;
    public bool finished;
    private List<GameState> gameStates = new List<GameState>();
    private int currentStateIndex = 0;
    public Slider timeSlider;
    private float currentStateStartTime;
    public int GetStateIndex => currentStateIndex;
    public GameStateSystem()
    {
        gameStates.AddRange(new GameState[] {
        new CarouselState(),
        new ChampionSelectionState(),
        new CombatPhaseState(),
        new ChampionSelectionState(),
        new ChampionSelectionState(),
        new ChampionSelectionState(),
        new ChampionSelectionState(),
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
        finished = currentState.IsFinished;
        currentDuration = currentState.duration;
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            currentState.IsFinished = true;
        }
        if (currentState.IsFinished)
        {
            currentState.duration = 10;
            currentState.IsFinished = false;
            ChangeState((currentStateIndex + 1));
            currentStateStartTime = Time.time;
            return;
        }
        GetCurrentState().OnUpdate();

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
        gameStates[currentStateIndex].duration = 3;
        gameStates[currentStateIndex].IsFinished = false;
        gameStates[currentStateIndex].OnEnterState();
        OnGameStateChanged?.Invoke(GetCurrentState());
    }
}