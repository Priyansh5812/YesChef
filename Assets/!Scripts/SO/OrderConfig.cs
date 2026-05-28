using UnityEngine;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(fileName = "OrderConfig", menuName = "Scriptable Objects/OrderConfig")]
public class OrderConfig : ScriptableObject
{
    [field : SerializeField , SerializedDictionary("ItemType","Points")] public SerializedDictionary<KitchenItemType , int> 
    ItemPointMapping
    {
        get; private set;
    }
    
    [field : SerializeField] public KitchenItem[] PossibleOrderItems
    {
        get; private set;
    }
    
    [field : SerializeField , Range(2 , 20)] public int OrderDispatchCheckFreq
    {
        get; private set;
    }

    [field : SerializeField] public OrderTime MaxOrderTime
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