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

    /*
         public GameStateSystem()
    {
        // Create the list of game states
        gameStates.AddRange(new GameState[] {
            new CarouselState(),
            new MinionsState(), // Round 1: 2-4
            new RandomUserState(), // Round 2: 1-3
            new CarouselState(), // Round 2: 4
            new RandomUserState(), // Round 2: 5-6
            new KrugsState(), // Round 2: 7
            new RandomUserState(), // Round 3: 1-3
            new CarouselState(), // Round 3: 4
            new RandomUserState(), // Round 3: 5-6
            new MurkWolvesState(), // Round 3: 7
            new RandomUserState(), // Round 4: 1-3
            new CarouselState(), // Round 4: 4
            new RandomUserState(), // Round 4: 5-6
            new AurelionDoomState(), // Round 4: 7
            new RandomUserState(), // Round 5: 1-3
            new CarouselState(), // Round 5: 4
            new RandomUserState(), // Round 5: 5-6
            new NemesisMorganaState(), // Round 5: 7
            new RandomUserState(), // Round 6: 1-3
            new CarouselState(), // Round 6: 4
            new RandomUserState(), // Round 6: 5-6
            new GiantCrabgotState(), // Round 6: 7
            new RandomUserState(), // Round 7: 1-3
            new CarouselState(), // Round 7: 4
            new RandomUserState(), // Round 7: 5-6
            new GiantCrabgotState() // Round 7: 7
        });
    }
         */



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
        GameObject[] enemies;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (currentState.IsFinished)
        {
            ChangeState((currentStateIndex + 1) % gameStates.Count);
        }
        gameStates[currentStateIndex].OnUpdate();

        if (currentState is CombatPhaseState)
        {
            if (enemies.Count() == 0 && GetCurrentState() is CombatPhaseState)
            {
                currentState.IsFinished = true;
            }
            return;
        }

        float timer = Time.time - currentStateStartTime;
        if (timer > currentState.duration)
        {
            timer = 0;
            currentState.IsFinished = true;
            
        }
        timeSlider.value = timer;
        timeSlider.maxValue = currentState.duration;



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