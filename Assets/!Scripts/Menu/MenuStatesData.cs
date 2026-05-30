using UnityEngine;
using UnityEngine.UI;
using TMPro;
[System.Serializable]
public struct MainMenuStateData
{
    public CanvasGroup cgMain;
    public Button btn_play;
    public Button btn_Quit;
}


[System.Serializable]
public struct GameStateData
{
    public CanvasGroup cgMain;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI score;
}


[System.Serializable]
public struct PauseMenuStateData
{
    public CanvasGroup cgMain;
    public Button btn_Resume;
    public Button btn_Quit;
}


[System.Serializable]
public struct GameEndStateData
{
    public CanvasGroup cgMain;
    public Button btn_restart;
    public Button btn_Quit;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highscoreText;
}