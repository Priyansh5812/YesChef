using UnityEngine;
using System;
public interface IContainer
{   

    public void UpdateReflection(ContainerReflectionSystem reflection);
    public void OpenContainer();
    public void CloseContainer();
    public bool TryAddItem(KitchenItem item , int targetIndex);
    public bool RemoveItem(int targetIndex);

    public bool IsOpened
    {
        get; 
    }
}
