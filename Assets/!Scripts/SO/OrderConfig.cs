using UnityEngine;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(fileName = "OrderConfig", menuName = "Scriptable Objects/OrderConfig")]
public class OrderConfig : ScriptableObject
{
    [field : SerializeField , SerializedDictionary("ItemType","Points")] public SerializedDictionary<KitchenItemType , uint> 
    PointConfig
    {
        get; private set;
    }
    
    [field : SerializeField] public KitchenItem[] possibleOrderItems
    {
        get; private set;
    }
    
    [field : SerializeField] public OrderTime maxOrderTime
    {
        get; private set;
    }
    
}

[System.Serializable]
public struct OrderTime
{
    public uint Mins;
    public uint Secs;
}