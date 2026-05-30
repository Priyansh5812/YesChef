using System;
using UnityEngine;
public class PauseMenuState : IMonoState
{
    readonly MenuStateDriver driver;
    readonly PauseMenuStateData data;

    CursorLockMode lastLockMode;

    // keep the driver and pause data together
    public PauseMenuState(MenuStateDriver driver, PauseMenuStateData data)
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
        // show the pause menu and freeze the flow
        InitListeners();
        ToggleView(true);
        EventManager.OnGamePaused.Invoke();
        lastLockMode = Cursor.lockState;
        Cursor.lockState = CursorLockMode.Confined;
        OnEnableCompleted?.Invoke();
    }

    void InitListeners()
    {
        // hook up resume and quit actions
        data.btn_Resume.onClick.AddListener(ResumeGame);
        data.btn_Quit.onClick.AddListener(driver.QuitGame);
    }

    public void Start(Action OnStartCompleted = null)
    {   
        // note that the pause screen has been triggered
        IsAlreadyTriggered = true;
        OnStartCompleted?.Invoke();
    }
    
    void ToggleView(bool isActive)
    {
        // show or hide the pause panel
        this.data.cgMain.alpha = isActive ? 1.0f : 0.0f;
        this.data.cgMain.interactable = this.data.cgMain.blocksRaycasts = isActive;
    }

    void ResumeGame()
    {       
        Cursor.lockState = lastLockMode;
        // return to gameplay from the pause screen
        this.driver.InitiateStateChange(typeof(GameState));
    }

    void DeInitListeners()
    {
        // remove pause menu button hooks
        data.btn_Resume.onClick.RemoveListener(ResumeGame);
        data.btn_Quit.onClick.RemoveListener(driver.QuitGame);
    }

    public void OnDisable(Action OnDisableCompleted = null)
    {   
        // hide the pause menu and continue the round
        DeInitListeners();
        ToggleView(false);
        EventManager.OnGameResumed.Invoke();
        OnDisableCompleted?.Invoke();
    }
}
