using System;
using UnityEngine;

// handles one container instance and its actions
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
        // keep the config and interactable together
        m_data = data;
        dataManager = new(this, m_data);
        this.associatedInteractable = interactable;
    }

    public void UpdateReflection(IContainerReflectionSystem reflection)
    {   
        // push the current container data into the reflection view
        reflection.ReflectContainer(dataManager.ContainerData , this);
    }

    public void OpenContainer()
    {
        // mark the container as open and notify listeners
        EventManager.OnContainerOpened.Invoke();
        IsOpened = true;
    }
    
    public void CloseContainer(bool byPassChecks = false)
    {   
        // respect locked actions and active drag operations
        if(!byPassChecks && IsContainerLocked && !m_data.IsFunctionPassive)
            return;

        if(!byPassChecks && IContainerReflectionSystem.IsUnderDragOperation)
            return;

        EventManager.OnContainerClosed.Invoke();
        IsOpened = false;
    }

    public bool ProcessContainerRequest(IContainerRequest req)
    {   
        // route the request to the matching data manager call
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

    public KitchenInteractable GetAssociatedInteractable() => this.associatedInteractable;

    #region Container Actions

    public void PerformAction()
    {   
        // run the action that matches the container setup
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
            case ContainerFunctionType.SERVE:
                ServeAction();
                break;
            case ContainerFunctionType.NONE:
            default:
                break;
        }
        
    }

    void DisposeAction()
    {   
        // clear every slot in the container
        for(int i = 0 ; i < dataManager.ContainerData.Length; i++)
        {
            dataManager.RemoveItem(i);
        }

        EventManager.RefreshContainerReflections?.Invoke();
    }

    void SetupTimedOperation()
    {   
        // start a timed action on the container
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
        // track the current action progress
        this.functionProgression = progress;
    }

    void OnTimedOperationCompletion()
    {   
        // apply the finished action to every item in the container
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

    void ServeAction()
    {
        // serve the current counter order if the items match
        var counter = this.associatedInteractable as CounterInteractable;

        if(counter == null)
        {
            Debug.LogError($"A Serve function was intended on an interactable which is not of CounterInteractable type\n Function Halted");
            return;
        }

        if(!((CounterInteractable)this.associatedInteractable).ValidateOrder(dataManager.ContainerData))
        {   
            Debug.LogError("Invalid Serve");
            return;
        }

        counter.ServeOrder();
        DisposeAction();
        CloseContainer();
    }
    
    #endregion

    public void GetConfigInfo(out string title, out ContainerFunctionType funcType)
    {
       // expose the config info to reflection views
       title = m_data.ContainerName;
       funcType = m_data.ContainerFunction;
    }

    public void GetFunctionCompletionStat(out float progression , out float completionTime)
    {
        // report the current action progress
        progression = this.functionProgression;
        completionTime = this.functionCompletionTime;
    }

    public Order GetCounterOrder()
    {
        // ask the counter for its current order
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

    public void ResetContainer()
    {
        // clear the container and stop any running action
        if(this.IsOpened)
            this.CloseContainer(true);
            
        if(IsContainerLocked)
        {
            this.associatedInteractable.StopAllCoroutines();
            IsContainerLocked = false;
            functionProgression = 0f; 
            functionCompletionTime = -1;
        }

        DisposeAction();
    }
}
