using System;
using UnityEngine;

// TODO : Separate the concerns of the Container Modification and Container Type Actions
public class SlotContainer : IContainer
{   
    ContainerConfig m_data;
    KitchenItem[] containerData;
    KitchenInteractable associatedInteractable;
    bool isContainerLocked = false;
    float functionProgression = 0;
    float functionCompletionTime = -1;

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
        if(isContainerLocked && !m_data.IsFunctionPassive)
            return;

        if(ContainerReflectionSystem.IsUnderDragOperation)
            return;

        EventManager.OnContainerClosed.Invoke();
        IsOpened = false;
    }

    #region Container Modification Related

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
    #endregion

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

    bool IsContainerEmpty()
    {
        bool isContainerEmpty = true;
        foreach(var i in containerData)
        {
            if(i != null)
            {
                isContainerEmpty = false;
                break;
            }
        }

        return isContainerEmpty;
    }

    bool IsContainerFunctionEligible()
    {   
        return true;
    }

    void SetupTimedOperation()
    {   

        if(IsContainerEmpty())
        {
            Debug.LogWarning("Halted beforehand");
            return;
        }

        if(!IsContainerFunctionEligible())
        {
            return;
        } 

        isContainerLocked = true;
        functionProgression = 0f; 
        functionCompletionTime = Time.time + m_data.FunctionCompletionDuration;
        associatedInteractable?.InitiateSlotFunctionTimer(m_data.FunctionCompletionDuration, OnTimedOperationUpdation , OnTimedOperationCompletion);
    }   

    void OnTimedOperationUpdation(float progress)
    {
        this.functionProgression = progress;
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
        functionProgression = 0f; 
        functionCompletionTime = -1;
    }

    #endregion
    public void GetConfigInfo(out string title, out ContainerFunctionType funcType)
    {
       title = m_data.ContainerName;
       funcType = m_data.ContainerFunction;
    }

    public void GetFunctionCompletionStat(out float progression , out float completionTime)
    {
        progression = this.functionProgression;
        completionTime = this.functionCompletionTime;
    }

    public Order GetCounterOrder()
    {
        if(this.associatedInteractable is CounterInteractable)
        {
            return ((CounterInteractable) this.associatedInteractable).GetOrder();
        }
        else
        {   
            Debug.LogError("An order request was intended from a Non-CounterInteractable reference. Returned NULL");
            return null;
        }
    }
}
