using UnityEngine;

// shared contract for reflection systems
public interface IContainerReflectionSystem
{   
    // tracks the active drag transfer
    public static KitchenItemTransferRequest? ActiveTransferRequest
    {
        get; set;
    }

    // tracks whether an item is being dragged
    public static bool IsUnderDragOperation
    {
        get;  set;
    } 

    // container currently shown in the reflection ui
    public IContainer associatedContainer
    {
        get;
    }

    // pushes container data into the reflection view
    public void ReflectContainer(KitchenItem[] items , IContainer container);
}
