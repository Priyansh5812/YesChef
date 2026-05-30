using System;
using UnityEngine;
public class GameEndState : IMonoState
{
    readonly MenuStateDriver driver;
    readonly GameEndStateData data;
    readonly GameConfig config;

    // keep the driver and result data together
    public GameEndState(MenuStateDriver driver, GameEndStateData data)
    {
        this.driver = driver;
        this.data = data;
    }

    public bool IsAlreadyTriggered
    {
        get; private set;
    }

    public void OnEnable(Action OnEnableCompleted = null)
    {   
        // prepare the results view and show it
        InitListeners();
        PrepareStats();
        ToggleView(true);
        Cursor.lockState = CursorLockMode.Confined;
        OnEnableCompleted?.Invoke();
    }

    void InitListeners()
    {
        // wire up restart and quit actions
        this.data.btn_restart.onClick.AddListener(RestartGame);
        this.data.btn_Quit.onClick.AddListener(driver.QuitGame);
    }

    public void Start(Action OnStartCompleted = null)
    {   
        // remember that the results screen is active
        IsAlreadyTriggered = true;
        OnStartCompleted?.Invoke();
    }

    void PrepareStats()
    {   
        // work out the new high score and update the labels
        int highScore;
        if(!PlayerPrefs.HasKey(Constants.HighScore))
        {
            // use the latest score when no record exists
            highScore = driver.SessionScore;
            
        }
        else
        {
            // keep the better score
            highScore = Mathf.Max(driver.SessionScore , PlayerPrefs.GetInt(Constants.HighScore));
        }

        PlayerPrefs.SetInt(Constants.HighScore , highScore);

        data.scoreText?.SetText($"Score : {driver.SessionScore}");
        data.highscoreText?.SetText($"Score : {highScore}");
    
    }

    void RestartGame()
    {
        // send the player back into gameplay
        driver.InitiateStateChange(typeof(GameState));
    }

    void ToggleView(bool isActive)
    {
        // show or hide the results panel
        this.data.cgMain.alpha = isActive ? 1.0f : 0.0f;
        this.data.cgMain.interactable = this.data.cgMain.blocksRaycasts = isActive;
    }
    

    void DeInitListeners()
    {
        // remove the results screen listeners
        this.data.btn_restart.onClick.RemoveListener(RestartGame);
        this.data.btn_Quit.onClick.RemoveListener(driver.QuitGame);
    }

    public void OnDisable(Action OnDisableCompleted = null)
    {   
        // clear the results screen before leaving
        ToggleView(false);
        DeInitListeners();
        OnDisableCompleted?.Invoke();
    }

}
