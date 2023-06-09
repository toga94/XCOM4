using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStateSystem : Singleton<GameStateSystem>
{
    public event Action<GameState> OnGameStateChanged;
    public float currentDuration;
    public bool finished;
    private List<List<GameState>> rounds = new List<List<GameState>>(10);
    [SerializeField]
    private int currentRoundIndex = 0;
    [SerializeField]
    private int currentStateIndex = 0;
    public Slider timeSlider;
    private float currentStateStartTime;

    public int GetRoundIndex => currentRoundIndex;
    public int GetStateIndex => currentStateIndex;
    public int GetCurrentStateIndexUI
    {
        get
        {
            if (currentRoundIndex >= 0 && currentRoundIndex < rounds.Count)
            {
                List<GameState> currentRound = rounds[currentRoundIndex];
                for (int i = currentStateIndex; i < currentRound.Count; i++)
                {
                    if (!(currentRound[i] is ChampionSelectionState))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
    }
    public GameState CurrentState => GetCurrentState;

    private List<GameState> gameStateList;
    public GameStateSystem()
    {
        gameStateList = new List<GameState>()
        {
            new CarouselState(),
            new ChampionSelectionState(),
            new Minion_1_1_PhaseState(),
            new ChampionSelectionState(),
            new Minion_1_1_PhaseState(),
            new ChampionSelectionState(),
            new CarouselState(),
            new ChampionSelectionState(),
            new PlayerCombat_PhaseState(),
            new ChampionSelectionState(),
            new PlayerCombat_PhaseState(),
            new ChampionSelectionState(),
            new PlayerCombat_PhaseState(),
            new ChampionSelectionState(),
        };
        for (int i = 0; i < 10; i++)
        {
            rounds.Add(new List<GameState>(gameStateList));
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
            float timer = Time.time - currentStateStartTime;
            if (timer > currentState.duration)
            {
                currentState.IsFinished = true;
            }

            timeSlider.value = timer;
            timeSlider.maxValue = currentState.duration;
            currentState.OnUpdate();
        }
    }
    public List<GameState> currentRound;
    public void NextState()
    {
        if (currentRoundIndex < rounds.Count)
        {
            currentRound = rounds[currentRoundIndex];
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
    public List<GameState> GetStatesInRound()
    {

        List<GameState> currentRound = rounds[currentRoundIndex];
        List<GameState> statesInRound = new List<GameState>();

        foreach (GameState state in currentRound)
        {
            if (!(state is ChampionSelectionState))
            {
                statesInRound.Add(state);
            }
        }

        return statesInRound;

    }
    public List<GameState> GetAllStatesInCurrentRound()
    {

        List<GameState> currentRound = rounds[currentRoundIndex];
        List<GameState> statesInRound = new List<GameState>();

        foreach (GameState state in currentRound)
        {
            statesInRound.Add(state);
        }

        return statesInRound;

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
    private int GetMaxStateInRound()
    {
        List<GameState> _curRound = rounds[currentRoundIndex];


        return 0;
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