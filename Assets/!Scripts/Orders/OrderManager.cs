using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
// handles order spawning scoring and cleanup
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
        // build the counter pools and listen for game events
        InitializeCounters();
        aux ??= ListPool<KitchenItem>.Get();
        InitListeners();
    }

    void InitializeCounters()
    {
        // cache all counters so orders can be routed quickly
        if(inactiveCounters == null)
        {
            inactiveCounters = ListPool<CounterInteractable>.Get();
            inactiveCounters.Capacity = 4; // keeps room for the counters
            inactiveCounters.AddRange(this.GetComponentsInChildren<CounterInteractable>());
        }

        activeCounters ??= HashSetPool<CounterInteractable>.Get();
    }

    void InitListeners()
    {   
        // react to the main game flow events
        EventManager.OnGameStarted.AddListener(InitiateOrderDispatch);
        EventManager.OnGameOver.AddListener(PerformCleanup);
        EventManager.OnGamePaused.AddListener(PauseOrderDispatch);
        EventManager.OnGameResumed.AddListener(ResumeOrderDispatch);
        EventManager.PreOrderServed.AddListener(ProcessServedOrder);
    }

    void InitiateOrderDispatch()
    {
        // start checking for new orders after a short delay
        InvokeRepeating(nameof(CheckForOrderDispatch),5.0f,orderConfig.OrderDispatchCheckFreq);
    }

    #region ORDER-PREP
    void PrepareOrderItems()
    {
        // build a fresh list of random items
        aux.Clear();
        int maxOrderItemCount = Random.Range(2 , 4);

        for(int i = 0; i < maxOrderItemCount; i++)
        {
            aux.Add(GetRandomKitchenItem());
        }
    }

    KitchenItem GetRandomKitchenItem()
    {
        // pick one item from the available order pool
        return orderConfig.PossibleOrderItems[Random.Range(0 , orderConfig.PossibleOrderItems.Length)];
    }
    #endregion

    void CheckForOrderDispatch()
    {   
        // stop if the game is paused or no counters are free
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
        // score the order and return the counter to the pool
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
        // pause live counters during the game pause screen
        IsPaused = true;
        foreach(var i in activeCounters)
            i.SetPauseRunningOrder(IsPaused);
    }

    void ResumeOrderDispatch()
    {
        // resume order timers when play returns
        IsPaused = false;
        foreach(var i in activeCounters)
            i.SetPauseRunningOrder(IsPaused);
    }

    void PerformCleanup()
    {
        // stop future order checks and reset the counters
        CancelInvoke(nameof(CheckForOrderDispatch));
        ResetCounters();
    }

    void ResetCounters()
    {
        // move all active counters back to the inactive list
        foreach(var i in activeCounters)
        {
            i.ClearOrder();
            inactiveCounters.Add(i);
        }
    }


    void DeInitListeners()
    {   
        // remove the shared event hooks before shutdown
        EventManager.OnGameStarted.RemoveListener(InitiateOrderDispatch);
        EventManager.OnGameOver.RemoveListener(PerformCleanup);
        EventManager.OnGamePaused.RemoveListener(PauseOrderDispatch);
        EventManager.OnGameResumed.RemoveListener(ResumeOrderDispatch);
        EventManager.PreOrderServed.RemoveListener(ProcessServedOrder);
    }

    void DeInitializeCounters()
    {
        // release the pooled counter collections
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
        // return pooled data and remove listeners
        if(aux != null)
        {
            ListPool<KitchenItem>.Release(aux);
            aux = null;
        }

        DeInitListeners();
        DeInitializeCounters();
    }

}
