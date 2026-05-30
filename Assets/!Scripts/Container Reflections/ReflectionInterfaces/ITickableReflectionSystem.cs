using UnityEngine;
using System.Collections;

// reflection systems that show an active timer
public interface ITickableReflectionSystem : IContainerReflectionSystem
{
    // update the action controls for the current container
    public void SetupFunctionAuthority(ContainerFunctionType functionType);
}
