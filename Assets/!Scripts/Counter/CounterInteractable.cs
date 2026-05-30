using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CounterInteractable : KitchenInteractable
{   

    [SerializeField] CounterViewData viewData;
    Order order;
    CounterView view;
    Coroutine orderTimerRoutine = null;
    float secsElapsedFromOrder;
    bool isPaused;

    protected override void OnEnable()
    {   
        base.OnEnable();
        order ??= new();
        order.Initialize();
    }

    protected override void Start()
    {   
        base.Start();
        view ??= new(this , this.viewData);
    }

#region ORDER
    public Order GetOrder() => this.order;

    public void DispatchOrder(List<KitchenItem> orderItems)
    {   
        ClearOrder();
        order?.AddItems(orderItems);
        InitializeOrderView();
        secsElapsedFromOrder = 0f;
        orderTimerRoutine = StartCoroutine(OrderTimerRoutine()); 
    }

    void InitializeOrderView()
    {
        view?.RefreshOrderView(this.order);
    }

    public bool ValidateOrder(KitchenItem[] items)
    {   
        int size = order.Count;

        if(size == 0)
            return false;

        for(int i = 0 ; i < size; i++)
        {
            if(!order.IsOrderItemEqual(items[i] , i))
                return false;    
        }
        return true;
    }

    public void ServeOrder()
    {
        EventManager.PreOrderServed.Invoke(new OrderServeData(this, this.order , this.secsElapsedFromOrder));
        ClearOrder();
    }

    public void ClearOrder()
    {   
        if(orderTimerRoutine != null)
        {
            StopCoroutine(orderTimerRoutine);
        }
        
        order?.ClearOrderItems();
        view.UpdateCounterTimer("--:--");
        view.ToggleOrderView(false);
       
    }
    
    IEnumerator OrderTimerRoutine()
    {
        while(true)
        {   
            while(isPaused)
            {
                yield return null;
            }

            secsElapsedFromOrder += Time.deltaTime;
            int floorSecs = Mathf.FloorToInt(secsElapsedFromOrder);
            int mins = floorSecs / 60;
            int secs = floorSecs % 60;
            view.UpdateCounterTimer($"{(mins < 10 ? $"0{mins}" : mins)}:{(secs < 10 ? $"0{secs}" : secs)}");
            yield return null;
        }
    }

    public void SetPauseRunningOrder(bool value)
    {
        isPaused = value;
    }


#endregion

    public void DisplayScore(int scoreAmt)
    {
        Debug.Log($"Score : +{scoreAmt}");
    }

    protected override void OnDisable()
    {   
        base.OnDisable();
        order?.Dispose();
    }
}
