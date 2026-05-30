using UnityEngine;
using System.Collections.Generic;
using System;
public class MenuStateDriver : MonoBehaviour
{
    // keeps each state ready for quick swaps
    readonly Dictionary<Type, IMonoState> stateReg = new();
    // menu data that each state reads from
    [SerializeField] MainMenuStateData mainMenuStateData;
    [SerializeField] GameStateData gameMenuStateData;
    [SerializeField] PauseMenuStateData pauseMenuStateData;
    [SerializeField] GameEndStateData gameEndStateData;

    [Header("Configs")]
    [SerializeField] GameConfig gameConfig;
    bool isChangingState;
    IMonoState currentState;

    public int SessionScore
    {
        get; set;
    }

    void Start()
    {
        // build the state table before opening the menu
        ConstructMenuStates();
        InitiateStateChange(typeof(MainMenuState));
    }

    void ConstructMenuStates()
    {
        // wire each state to its matching data
        stateReg.Clear();
        stateReg.Add(typeof(MainMenuState), new MainMenuState(this, mainMenuStateData));
        stateReg.Add(typeof(GameState), new GameState(this, gameMenuStateData, gameConfig));
        stateReg.Add(typeof(PauseMenuState), new PauseMenuState(this, pauseMenuStateData));
        stateReg.Add(typeof(GameEndState), new GameEndState(this, gameEndStateData));
    }
    
    public void InitiateStateChange(Type newStateType)
    {
        if (isChangingState)
        {
            // wait until the current switch is done
            return;
        }

        IMonoState newState;
        if (stateReg.ContainsKey(newStateType))
        {
            // find the next state by its type
            newState = stateReg[newStateType];
        }
        else
        {
            Debug.LogError($"Unknown {newStateType} state type");
            return;
        }

        isChangingState = true;
        if (currentState != null)
        {
            // let the active state clean itself up first
            currentState.OnDisable(OnInitialCompleted);
        }
        else
        {
            OnInitialCompleted();
        }


        void OnInitialCompleted()
        {
            // move into the new state once cleanup is done
            currentState = newState;

            if (currentState.IsAlreadyTriggered)
            {
                currentState.OnEnable(OnEnableCompleted);
            }
            else
            {
                currentState.OnEnable(OnEnableThenStart);
            }
        }

        void OnEnableThenStart()
        {
            // start the state after enable work is finished
            currentState.Start(OnStartCompleted);
        }

        void OnEnableCompleted()
        {
            // release the transition lock after enable
            isChangingState = false;
        }

        void OnStartCompleted()
        {
            // release the transition lock after start
            isChangingState = false;
        }
    }

    #region Common State Methods

    public void QuitGame()
    {   
        // quit play mode in the editor and exit the build at runtime
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #endregion

    void OnDisable()
    {
        // let the current state shut down with the driver
        currentState?.OnDisable();
    }

}
