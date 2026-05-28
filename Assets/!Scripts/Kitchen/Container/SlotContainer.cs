using System;
using UnityEngine;

// TODO : Separate the concerns of the Container Modification and Container Type Actions
public class SlotContainer : IContainer
{   
    ContainerConfig m_data;
    ContainerDataManager dataManager;
    KitchenInteractable associatedInteractable;
    public bool IsContainerLocked
    {
        get; set;
    }
    float functionProgression = 0;
    float functionCompletionTime = -1;

    public bool IsOpened
    {
        get; private set;
    } = false;

    public SlotContainer(ContainerConfig data , KitchenInteractable interactable)
    {
        m_data = data;
        dataManager = new(this, m_data);
        this.associatedInteractable = interactable;
    }



    public void UpdateReflection(ContainerReflectionSystem reflection)
    {   
        reflection.ReflectContainer(dataManager.ContainerData , this);
    }

    public void OpenContainer()
    {
        EventManager.OnContainerOpened.Invoke();
        IsOpened = true;
    }
    
    public void CloseContainer()
    {   
        if(IsContainerLocked && !m_data.IsFunctionPassive)
            return;

        if(ContainerReflectionSystem.IsUnderDragOperation)
            return;

        EventManager.OnContainerClosed.Invoke();
        IsOpened = false;
    }



    #region Container Actions

    public void PerformAction()
    {   
        if(IsContainerLocked)
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
        for(int i = 0 ; i < dataManager.ContainerData.Length; i++)
        {
            dataManager.RemoveItem(i);
        }

        EventManager.RefreshContainerReflections?.Invoke();
    }

    void SetupTimedOperation()
    {   

        if(dataManager.IsContainerEmpty())
        {
            Debug.LogWarning("Halted beforehand");
            return;
        }

        IsContainerLocked = true;
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
        for(int i = 0 ; i < this.dataManager.ContainerData.Length; i++)
        {   
            if(this.dataManager.ContainerData[i] == null)
                continue;

            if(m_data.ModifiedItemSprites.ContainsKey(this.dataManager.ContainerData[i].itemType))
            {
                this.dataManager.ContainerData[i].sprite = m_data.ModifiedItemSprites[this.dataManager.ContainerData[i].itemType];
                switch(m_data.ContainerFunction)
                {
                    case ContainerFunctionType.SLICE:
                        this.dataManager.ContainerData[i].isChopped = true;
                        break;
                    case ContainerFunctionType.COOK:
                        this.dataManager.ContainerData[i].isCooked = true;
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
        IsContainerLocked = false;
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

    public bool ProcessContainerRequest(IContainerRequest req)
    {   
        switch(req)
        {
            case ContainerAddItem obj:
                return dataManager.AddItem(obj.item , obj.targetIndex);
            case ContainerRemoveItem obj:
                return dataManager.RemoveItem(obj.targetIndex);
            case ContainerEvaluateItemAddition obj:
                return dataManager.EvaluateItemAddition(obj.item , obj.targetIndex);
            case ContainerEvaluateItemRemoval obj:
                return dataManager.EvaluateItemRemoval(obj.targetIndex);
            default:
                return false;
        }
    }
}
