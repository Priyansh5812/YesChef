using UnityEngine;
using System;
public interface IContainer
{   
    public void UpdateReflection(ContainerReflectionSystem reflection);
    public void OpenContainer();
    public void CloseContainer();
    public bool ProcessContainerRequest(IContainerRequest req);
    public void PerformAction();
    public void GetConfigInfo(out string title , out ContainerFunctionType funcType);
    public void GetFunctionCompletionStat(out float progression , out float completionTime);
    public Order GetCounterOrder();
    public void ResetContainer();

    public bool IsOpened
    {
        get; 
    }
    public bool IsContainerLocked
    {
        get; 
    }

}
