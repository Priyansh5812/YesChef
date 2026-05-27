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
    public void PerformAction();
    public void GetConfigInfo(out string title , out ContainerFunctionType funcType);
    public void GetFunctionCompletionStat(out float progression , out float completionTime);
    public bool IsOpened
    {
        get; 
    }

}
