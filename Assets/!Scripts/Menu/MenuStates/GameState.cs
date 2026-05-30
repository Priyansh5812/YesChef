using System;
using System.Collections;
using UnityEngine;
public class GameState : IMonoState
{
    readonly MenuStateDriver driver;
    readonly GameStateData data;
    readonly GameConfig config;
    bool isGamePaused;
    float remainingSecs;
    int score;
    

    // keep the play session pieces together
    public GameState(MenuStateDriver driver, GameStateData data, GameConfig config)
    {
        this.driver = driver;
        this.data = data;
        this.config = config;
    }

    public bool IsAlreadyTriggered
    {
        get; private set;
    }

    public void OnEnable(Action OnEnableCompleted = null)
    {   
        // get the gameplay view and listeners ready
        InitListeners();
        PrepareStartup();
        OnEnableCompleted?.Invoke();
    }

    void InitListeners()
    {   
        // watch for score updates while the round is live
        EventManager.OnOrderServed.AddListener(UpdateScore);
    }

    public void Start(Action OnStartCompleted = null)
    {   
        // remember that gameplay has already started
        IsAlreadyTriggered = true;
        OnStartCompleted?.Invoke();
    }

    void PrepareStartup()
    {   
        // reset the round and either resume or start fresh
        ToggleView(true);
        
        if(isGamePaused)
        {
            // only clear the pause flag when resuming
            isGamePaused = false;
        }
        else
        {   
            // start a fresh round and clear the old score
            remainingSecs = TimeSpan.TimeSpanToSecs(config.GameTime);
            UpdateScore(-score); // resets the score
            EventManager.OnGameStarted.Invoke();
            driver.StartCoroutine(GameTimer());    
        }
    }
    

    void ToggleView(bool isActive)
    {
        // show or hide the gameplay screen
        this.data.cgMain.alpha = isActive ? 1.0f : 0.0f;
        this.data.cgMain.interactable = this.data.cgMain.blocksRaycasts = isActive;
    }

    IEnumerator GameTimer()
    {   
        // tick the round down one frame at a time
        while(remainingSecs > 0)
        {
            remainingSecs -= Time.deltaTime;
            remainingSecs = remainingSecs < 0 ? 0 : remainingSecs;

            int floorSecs = Mathf.FloorToInt(remainingSecs);
            var span = TimeSpan.SecsToTimeSpan(floorSecs);

            UpdateTimerView($"{(span.Mins < 10 ? $"0{span.Mins}" : span.Mins)}:{(span.Secs < 10 ? $"0{span.Secs}" : span.Secs)}");


            CheckForGamePause();

            while(isGamePaused)
            {
                // hold the timer until play resumes
                yield return null;
            }

            yield return null;
        }

        // store the final score and move to the results screen
        driver.SessionScore = score;
        EventManager.OnGameOver.Invoke();
        driver.InitiateStateChange(typeof(GameEndState));
    }

    void CheckForGamePause()
    {
        // open the pause screen when escape is pressed
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            isGamePaused = true;
            driver.InitiateStateChange(typeof(PauseMenuState));
        }
    }
    

    void UpdateTimerView(string str)
    {
        // refresh the timer text
        this.data.timerText?.SetText(str);
    }

    void UpdateScore(int modifier)
    {
        // add to the score and refresh the display
        score += modifier;
        UpdateScoreView();
    }

    void UpdateScoreView()
    {
        // show the current score
        data.score?.SetText(score.ToString());
    }

    void DeInitListeners()
    {   
        // stop listening for score changes
        EventManager.OnOrderServed.RemoveListener(UpdateScore);
    }

    public void OnDisable(Action OnDisableCompleted = null)
    {   
        // clean up before leaving gameplay
        DeInitListeners();
        OnDisableCompleted?.Invoke();
    }
}
