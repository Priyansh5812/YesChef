using UnityEngine;
using System;
public interface IContainer
{   

    public void UpdateReflection(ContainerReflectionSystem reflection);
    public void OpenContainer();
    public void CloseContainer();
    public void AddItem(KitchenItem item , int targetIndex);
    public void RemoveItem(int targetIndex);
    public bool EvaluateItemAddition(KitchenItem item , int targetIndex);
    public bool EvaluateItemRemoval(int targetIndex);

    public bool IsOpened
    {
        get; 
    }
}
