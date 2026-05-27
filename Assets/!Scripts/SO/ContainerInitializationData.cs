using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "ContainerConfig", menuName = "Scriptable Objects/ContainerConfig")]
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
    [field : SerializeField , Range(1 , 5)] public int FallbackSlotCount
    {
        get; private set;   
    }
}

public enum ContainerFunctionType
{
    NONE,
    SLICE,
    COOK,
    DISPOSE
}
