using TMPro;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public struct CounterViewData
{
    // ui pieces used by the counter view
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI pointsText;
    public CanvasGroup cgMain;
    public Image[] orderImages;
    public Color goodScoreColor , badScoreColor;
}
