using UnityEngine;
using UnityEngine.UI;
using TMPro;
[System.Serializable]
public struct MainMenuStateData
{
    // references for the main menu screen
    public CanvasGroup cgMain;
    public Button btn_play;
    public Button btn_Quit;
}


[System.Serializable]
public struct GameStateData
{
    // references for the game screen
    public CanvasGroup cgMain;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI score;
}


[System.Serializable]
public struct PauseMenuStateData
{
    // references for the pause screen
    public CanvasGroup cgMain;
    public Button btn_Resume;
    public Button btn_Quit;
}


[System.Serializable]
public struct GameEndStateData
{
    // references for the results screen
    public CanvasGroup cgMain;
    public Button btn_restart;
    public Button btn_Quit;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highscoreText;
}
