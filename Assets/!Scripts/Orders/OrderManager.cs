using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class OrderManager : MonoBehaviour
{   
    [SerializeField] OrderConfig orderConfig;
    List<CounterInteractable> inactiveCounters;
    HashSet<CounterInteractable> activeCounters;
    List<KitchenItem> aux;

    public bool IsPaused
    {
        get; private set;
    }

    void OnEnable()
    {   
        InitializeCounters();
        aux ??= ListPool<KitchenItem>.Get();
        InitListeners();
    }

    void InitializeCounters()
    {
        if(inactiveCounters == null)
        {
            inactiveCounters = ListPool<CounterInteractable>.Get();
            inactiveCounters.Capacity = 4; // Reserving the memory
            inactiveCounters.AddRange(this.GetComponentsInChildren<CounterInteractable>());
        }

        activeCounters ??= HashSetPool<CounterInteractable>.Get();
    }

    void InitListeners()
    {   
        EventManager.OnGameStarted.AddListener(InitiateOrderDispatch);
        EventManager.OnGameOver.AddListener(PerformCleanup);
        EventManager.OnGamePaused.AddListener(PauseOrderDispatch);
        EventManager.OnGameResumed.AddListener(ResumeOrderDispatch);
        EventManager.PreOrderServed.AddListener(ProcessServedOrder);
    }

    void InitiateOrderDispatch()
    {
        InvokeRepeating(nameof(CheckForOrderDispatch),5.0f,orderConfig.OrderDispatchCheckFreq);
    }

    #region ORDER-PREP
    void PrepareOrderItems()
    {
        aux.Clear();
        int maxOrderItemCount = Random.Range(2 , 4);

        for(int i = 0; i < maxOrderItemCount; i++)
        {
            aux.Add(GetRandomKitchenItem());
        }
    }

    KitchenItem GetRandomKitchenItem()
    {
        return orderConfig.PossibleOrderItems[Random.Range(0 , orderConfig.PossibleOrderItems.Length)];
    }
    #endregion

    void CheckForOrderDispatch()
    {   
        if(IsPaused)
            return;

         if(inactiveCounters.Count == 0)
            return;

        int index = Random.Range(0 , inactiveCounters.Count);
        var counter = inactiveCounters[index];
        inactiveCounters.RemoveAt(index);
        PrepareOrderItems();
        activeCounters.Add(counter);
        counter.DispatchOrder(aux);
    }

    
    void ProcessServedOrder(OrderServeData data)
    {
        int score = 0;
        aux.Clear();
        data.order.GetOrderItems(aux);
        foreach(var i in aux)
        {   
            if(!orderConfig.ItemPointMapping.ContainsKey(i.itemType))
            {   
                Debug.LogWarning($"Point for ItemType {i.itemType} is not registered inside the OrderConfig!\n Point addition skipped.");
                continue;
            }
            score += orderConfig.ItemPointMapping[i.itemType];
        }

        score -= Mathf.FloorToInt(data.timeElapsed);
        EventManager.OnOrderServed.Invoke(score);
        data.counter.DisplayScore(score);
        inactiveCounters.Add(data.counter);
        activeCounters.Remove(data.counter);
    }

    void PauseOrderDispatch()
    {
        IsPaused = true;
        foreach(var i in activeCounters)
            i.SetPauseRunningOrder(IsPaused);
    }

    void ResumeOrderDispatch()
    {
        IsPaused = false;
        foreach(var i in activeCounters)
            i.SetPauseRunningOrder(IsPaused);
    }

    void PerformCleanup()
    {
        CancelInvoke(nameof(CheckForOrderDispatch));
        ResetCounters();
    }

    void ResetCounters()
    {
        foreach(var i in activeCounters)
        {
            i.ClearOrder();
            inactiveCounters.Add(i);
        }
    }


    void DeInitListeners()
    {   
        EventManager.OnGameStarted.RemoveListener(InitiateOrderDispatch);
        EventManager.OnGameOver.RemoveListener(PerformCleanup);
        EventManager.OnGamePaused.RemoveListener(PauseOrderDispatch);
        EventManager.OnGameResumed.RemoveListener(ResumeOrderDispatch);
        EventManager.PreOrderServed.RemoveListener(ProcessServedOrder);
    }

    void DeInitializeCounters()
    {
        if(inactiveCounters != null)
        {
            ListPool<CounterInteractable>.Release(inactiveCounters);
        }

        if(activeCounters != null)
        {
            HashSetPool<CounterInteractable>.Release(activeCounters);
        }
        
    }

    void OnDisable()
    {
        if(aux != null)
        {
            ListPool<KitchenItem>.Release(aux);
            aux = null;
        }

        DeInitListeners();
        DeInitializeCounters();
    }

}
