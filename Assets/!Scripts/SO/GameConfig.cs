using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Scriptable Objects/GameConfig")]
public class GameConfig : ScriptableObject
{
    [field : SerializeField] public TimeSpan GameTime
    {
        get; private set;
    }
}
