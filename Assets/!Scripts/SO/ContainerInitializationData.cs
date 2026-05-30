using UnityEngine;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
[CreateAssetMenu(fileName = "ContainerConfig", menuName = "Scriptable Objects/ContainerConfig")]
// container setup and interaction rules
public class ContainerConfig : ScriptableObject
{   
    [field : SerializeField] public string ContainerName
    {
        get; private set;   
    }
    public List<KitchenItem> InitItems;
    [field : SerializeField] public ContainerFunctionType ContainerFunction
    {
        get; private set;   
    }

    [field : SerializeField , Range(1f , 10f)] public float FunctionCompletionDuration
    {
        get; private set;   
    } = 2f;

    [field : SerializeField] public bool IsFunctionPassive
    {
        get; private set;   
    }

    [field: Space(10)]

    [field : SerializeField] public bool CanRestockFromConfig
    {
        get; private set;   
    }
    [field : SerializeField] public bool ItemAdditionCompatible
    {
        get; private set;   
    } = true;
    
    [field : SerializeField] public bool ItemRemovalCompatible
    {
        get; private set;   
    } = true;

    [field : SerializeField] public bool CheckItemForOrderEligibility
    {
        get; private set;
    }
    
    [field : SerializeField , Range(1 , 5)] public int FallbackSlotCount
    {
        get; private set;   
    }

    [field : SerializeField] public KitchenItemType[] IgnoreItemAdditionTypes
    {
        get; private set;
    }

    [field : SerializeField , SerializedDictionary("KitchenItemType","Sprite")] public SerializedDictionary<KitchenItemType , Sprite> ModifiedItemSprites
    {
        get; private set;
    }
    

}

public enum ContainerFunctionType
{
    // the actions a container can perform
    NONE,
    SLICE,
    COOK,
    DISPOSE,
    SERVE
}

