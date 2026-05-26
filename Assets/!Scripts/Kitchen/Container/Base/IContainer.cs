using UnityEngine;
using System;
public interface IContainer
{
    public void Initialize();
    public void OpenContainer(ContainerReflectionSystem reflection);
    public void CloseContainer(ContainerReflectionSystem reflection);
    public bool IsOpened
    {
        get; 
    }
}
