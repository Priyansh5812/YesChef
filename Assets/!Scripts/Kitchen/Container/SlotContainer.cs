using System;
using UnityEngine;

public class SlotContainer : IContainer
{   
    ContainerConfig m_data;
    KitchenItem[] containerData;
    KitchenInteractable associatedInteractable;

    bool isContainerLocked = false;

    public bool IsOpened
    {
        get; private set;
    } = false;

    public SlotContainer(ContainerConfig data , KitchenInteractable interactable)
    {
        m_data = data;
        this.associatedInteractable = interactable;
        Initialize();
    }

    void Initialize()
    {   
        if(m_data.InitItems.Count == 0)
        {
            containerData = new KitchenItem[m_data.FallbackSlotCount];
            return;
        }


       containerData = new KitchenItem[m_data.InitItems.Count];
       for(int i = 0 ; i < containerData.Length; i++)
        {
            this.Restock(i);
        }
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
        containerData[targetIndex] = null;
        
        if(m_data.CanRestockFromConfig)
            Restock(targetIndex);
    }

    

    void Restock(int restockIndex)
    {
        containerData[restockIndex] = new KitchenItem(m_data.InitItems[restockIndex]);
    }

    public bool EvaluateItemAddition(KitchenItem item, int targetIndex)
    {   
        if(isContainerLocked)
            return false;

        if(!m_data.ItemAdditionCompatible)
            return false;

        if(item == null)
            return false;

        if(targetIndex >= containerData.Length)
            return false;

        foreach(var i in m_data.IgnoreItemAdditionTypes)
        {
            if(i == item.itemType)
                return false;
        }

        return true;
    }

    public bool EvaluateItemRemoval(int targetIndex)
    {   
        if(isContainerLocked)
            return false;

        if(!m_data.ItemRemovalCompatible)
            return false;

        if(targetIndex >= containerData.Length)
            return false;

        return true;
    }

    #region Container Actions

    public void PerformAction()
    {   
        if(isContainerLocked)
            return;

        switch(m_data.ContainerFunction)
        {
            case ContainerFunctionType.SLICE:
            case ContainerFunctionType.COOK:
                SetupTimedOperation();
                break;
            case ContainerFunctionType.DISPOSE:
                DisposeAction();
                break;
            case ContainerFunctionType.NONE:
            default:
                break;
        }
        
    }

    void DisposeAction()
    {   
        for(int i = 0 ; i < this.containerData.Length; i++)
        {
            RemoveItem(i);
        }

        EventManager.RefreshContainerReflections?.Invoke();
    }

    void SetupTimedOperation()
    {   
        bool isInteractableEmpty = true;
        foreach(var i in containerData)
        {
            if(i != null)
            {
                isInteractableEmpty = false;
                break;
            }
        }

        if(isInteractableEmpty)
        {
            Debug.LogWarning("Halted beforehand");
            return;
        }

        isContainerLocked = true;
        associatedInteractable?.InitiateSlotFunctionTimer(5 , OnTimedOperationCompletion);
    }


    void OnTimedOperationCompletion()
    {   
        for(int i = 0 ; i < this.containerData.Length; i++)
        {   
            if(this.containerData[i] == null)
                continue;

            if(m_data.ModifiedItemSprites.ContainsKey(this.containerData[i].itemType))
            {
                this.containerData[i].sprite = m_data.ModifiedItemSprites[this.containerData[i].itemType];
                switch(m_data.ContainerFunction)
                {
                    case ContainerFunctionType.SLICE:
                        this.containerData[i].isChopped = true;
                        break;
                    case ContainerFunctionType.COOK:
                        this.containerData[i].isCooked = true;
                        break;
                    case ContainerFunctionType.DISPOSE:
                    case ContainerFunctionType.NONE:
                    default:
                        break;
                }
                
            }
        }

        if(IsOpened)
            EventManager.RefreshContainerReflections?.Invoke();
        isContainerLocked = false;
    }


    #endregion
    public void GetConfigInfo(out string title, out ContainerFunctionType funcType)
    {
       title = m_data.ContainerName;
       funcType = m_data.ContainerFunction;
    }

}
