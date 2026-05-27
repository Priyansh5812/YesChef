using System;
using UnityEngine;

public class SlotContainer : IContainer
{   
    protected ContainerConfig m_data;
    readonly int fallbackInitializationSize = 2;
    protected KitchenItem[] containerData;

    public bool IsOpened
    {
        get; private set;
    } = false;

    public SlotContainer(ContainerConfig data)
    {
        m_data = data;
        Initialize();
    }

    void Initialize()
    {
        if(m_data == null || m_data.InitItems.Count == 0)
        {
            containerData = new KitchenItem[fallbackInitializationSize];
            return;
        }

        containerData = m_data.InitItems.ToArray();
    }

    public void UpdateReflection(ContainerReflectionSystem reflection)
    {
        reflection.ReflectContainer(containerData , this);
    }

    public void OpenContainer()
    {
        EventManager.OnContainerOpened.Invoke();
        IsOpened = true;
    }
    
    public void CloseContainer()
    {
        EventManager.OnContainerClosed.Invoke();
        IsOpened = false;
    }

    public bool TryAddItem(KitchenItem item, int targetIndex)
    {   
        if(!m_data.ItemAdditionCompatible)
            return false;

        if(item == null)
            return false;

        if(targetIndex >= containerData.Length)
            return false;

        if(containerData[targetIndex] == null) // TODO : Will be different for other types of containers
        {
            this.containerData[targetIndex] = item;
            return true;
        }

        return false;
    }

    public bool RemoveItem(int targetIndex)
    {   
        if(!m_data.ItemRemovalCompatible)
            return false;

        if(targetIndex >= containerData.Length)
            return false;

        Debug.Log("Before Remove:");

        foreach(var i in containerData)
        {
            Debug.Log(i == null ? "NULL" : i.itemType);
        }

        containerData[targetIndex] = null;

        Debug.Log("After Remove:");

        foreach(var i in containerData)
        {
            Debug.Log(i == null ? "NULL" : i.itemType);
        }

        if(m_data.CanRestockFromConfig)
            Restock(targetIndex);

        return true;
    }

    void Restock(int restockIndex)
    {
        containerData[restockIndex] = new KitchenItem(m_data.InitItems[restockIndex]);
    }
}
