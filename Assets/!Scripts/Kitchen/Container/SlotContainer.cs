using System;
using UnityEngine;

public class SlotContainer : IContainer
{   
    protected ContainerConfig m_data;
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
        if(m_data.InitItems.Count == 0)
        {
            containerData = new KitchenItem[m_data.FallbackSlotCount];
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

    public void AddItem(KitchenItem item, int targetIndex)
    {   
        Debug.Log("Before Add:");

        foreach(var i in containerData)
        {
            Debug.Log(i == null ? "NULL" : i.itemType);
        }

        if(containerData[targetIndex] == null) // TODO : Will be different for other types of containers
        {
            this.containerData[targetIndex] = item;

            Debug.Log("After Add:");

            foreach(var i in containerData)
            {
                Debug.Log(i == null ? "NULL" : i.itemType);
            }

        }
    }

    public void RemoveItem(int targetIndex)
    {   
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
    }

    

    void Restock(int restockIndex)
    {
        containerData[restockIndex] = new KitchenItem(m_data.InitItems[restockIndex]);
    }

    public bool EvaluateItemAddition(KitchenItem item, int targetIndex)
    {   
        if(!m_data.ItemAdditionCompatible)
            return false;

        if(item == null)
            return false;

        if(targetIndex >= containerData.Length)
            return false;


        return true;
    }

    public bool EvaluateItemRemoval(int targetIndex)
    {   
        if(!m_data.ItemRemovalCompatible)
            return false;

        if(targetIndex >= containerData.Length)
            return false;

        return true;
    }

    #region Container Actions

    public void PerformAction()
    {
        
    }

    public void GetConfigInfo(out string title, out ContainerFunctionType funcType)
    {
       title = m_data.ContainerName;
       funcType = m_data.ContainerFunction;
    }

    #endregion
}
