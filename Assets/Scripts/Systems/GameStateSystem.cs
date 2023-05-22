using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStateSystem : Singleton<GameStateSystem>
{
    public event Action<GameState> OnGameStateChanged;
    public float currentDuration;
    public bool finished;
    private List<List<GameState>> rounds = new List<List<GameState>>();
    [SerializeField]
    private int currentRoundIndex = 0;
    [SerializeField]
    private int currentStateIndex = 0;
    public Slider timeSlider;
    private float currentStateStartTime;

    public int GetRoundIndex => currentRoundIndex;
    public int GetStateIndex => currentStateIndex;
    public GameState CurrentState => GetCurrentState;

    public GameStateSystem()
    {
        for (int i = 0; i < 10; i++)
        {
            rounds.Add(new List<GameState> {
            new CarouselState(),
            new ChampionSelectionState(),
            new Minion_1_1_PhaseState(),
            new ChampionSelectionState(),
            new Minion_1_1_PhaseState(),
            new ChampionSelectionState(),
            new Minion_1_1_PhaseState(),
        });
        }
    }

    private void Start()
    {
        InitializeRound();
    }

    private void InitializeRound()
    {
        if (currentRoundIndex < rounds.Count)
        {
            List<GameState> currentRound = rounds[currentRoundIndex];
            currentRound[currentStateIndex].OnEnterState();
        }
    }

    private void Update()
    {
        if (currentRoundIndex < rounds.Count)
        {
            List<GameState> currentRound = rounds[currentRoundIndex];
            GameState currentState = currentRound[currentStateIndex];
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
                NextState();
                currentStateStartTime = Time.time;
                return;
            }

            currentState.OnUpdate();

            float timer = Time.time - currentStateStartTime;
            if (timer > currentState.duration)
            {
                currentState.IsFinished = true;
            }

            timeSlider.value = timer;
            timeSlider.maxValue = currentState.duration;
        }
    }

    public void NextState()
    {
        if (currentRoundIndex < rounds.Count)
        {
            List<GameState> currentRound = rounds[currentRoundIndex];
            int nextStateIndex = currentStateIndex + 1;

            if (nextStateIndex >= 0 && nextStateIndex < currentRound.Count)
            {
                ChangeState(currentRound, nextStateIndex);
            }
            else
            {
                NextRound();
            }
        }
    }

    private void NextRound()
    {
        currentRoundIndex++;
        currentStateIndex = 0;

        if (currentRoundIndex < rounds.Count)
        {
            List<GameState> currentRound = rounds[currentRoundIndex];
            ChangeState(currentRound, currentStateIndex);
        }
        else
        {
            // Handle end of rounds
            Debug.Log("End of rounds");
        }
    }

    private void ChangeState(List<GameState> round, int index)
    {
        GameState currentState = GetCurrentState;
        round[currentStateIndex].OnExitState();

        currentStateIndex = index;
        round[currentStateIndex].duration = 3;
        round[currentStateIndex].IsFinished = false;
        round[currentStateIndex].OnEnterState();
        OnGameStateChanged?.Invoke(CurrentState);
    }

    public GameState GetCurrentState
    {
        get
        {
            if (currentRoundIndex >= 0 && currentRoundIndex < rounds.Count)
            {
                List<GameState> currentRound = rounds[currentRoundIndex];
                if (currentStateIndex >= 0 && currentStateIndex < currentRound.Count)
                {
                    return currentRound[currentStateIndex];
                }
            }
            return null;
        }
    }
}