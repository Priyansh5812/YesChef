using System;
using UnityEngine;

public class MainMenuState : IMonoState
{
    readonly MenuStateDriver driver;
    readonly MainMenuStateData data;

    // keep the driver and menu data close by
    public MainMenuState(MenuStateDriver driver, MainMenuStateData data)
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
        // show the menu and hook up the buttons
        Cursor.lockState = CursorLockMode.Confined;
        InitListeners();
        ToggleView(true);
        OnEnableCompleted?.Invoke();
    }

    public void Start(Action OnStartCompleted = null)
    {           
        // mark this screen as already shown once
        IsAlreadyTriggered = true;
        OnStartCompleted?.Invoke();
    }

    void InitListeners()
    {
        // listen for the button clicks that drive this menu
        this.data.btn_play.onClick.AddListener(StartGame);
        this.data.btn_Quit.onClick.AddListener(this.driver.QuitGame);
    }

    void StartGame()
    {
        // move the player into gameplay
        driver.InitiateStateChange(typeof(GameState));
    }


    void ToggleView(bool isActive)
    {
        // turn the main menu panel on or off
        this.data.cgMain.alpha = isActive ? 1.0f : 0.0f;
        this.data.cgMain.interactable = this.data.cgMain.blocksRaycasts = isActive;
    }

    void DeInitListeners()
    {
        // remove the listeners before leaving the screen
        this.data.btn_play.onClick.RemoveListener(StartGame);
        this.data.btn_Quit.onClick.RemoveListener(this.driver.QuitGame);
    }

    public void OnDisable(Action OnDisableCompleted = null)
    {   
        // hide the menu before another state takes over
        ToggleView(false);
        DeInitListeners();
        OnDisableCompleted?.Invoke();
    }
}
