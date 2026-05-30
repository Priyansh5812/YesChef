using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Scriptable Objects/GameConfig")]
// round timing settings for the menu flow
public class GameConfig : ScriptableObject
{
    [field : SerializeField] public TimeSpan GameTime
    {
        get; private set;
    }
}
