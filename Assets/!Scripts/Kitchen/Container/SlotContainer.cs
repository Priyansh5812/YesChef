using System;
using UnityEngine;

public class SlotContainer : IContainer
{   
    ContainerInitializationData m_data;
    readonly int fallbackInitializationSize = 2;
    protected KitchenItem[] containerData;

    public bool IsOpened
    {
        get; private set;
    } = false;

    public SlotContainer(ContainerInitializationData data , int fallback)
    {
        m_data = data;
        fallbackInitializationSize = fallback;
    }

    public virtual void Initialize()
    {
        if(m_data == null)
        {
            containerData = new KitchenItem[Mathf.Max(2 , fallbackInitializationSize)];
            return;
        }

        containerData = m_data.initItems.ToArray();
        return;
    }

    public void OpenContainer(ContainerReflectionSystem reflection)
    {
        reflection.PopulateContainer(containerData);
        EventManager.OnPreContainerOpened.Invoke();
        EventManager.OnContainerOpened.Invoke();
        IsOpened = true;
    }
    
    public void CloseContainer(ContainerReflectionSystem reflection)
    {
        EventManager.OnContainerClosed.Invoke();
        IsOpened = false;
    }
    
}
