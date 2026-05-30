using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
// manages counter orders and the counter view
public class CounterInteractable : KitchenInteractable
{   

    [SerializeField] CounterViewData viewData;
    [SerializeField, Range(1f , 5f)] float ScoreDisplayDuration = 2.0f;

    Order order;
    CounterView view;
    Coroutine orderTimerRoutine = null;
    float secsElapsedFromOrder;
    bool isPaused;
    bool isDisplayingScore;


    protected override void OnEnable()
    {   
        // prepare an order object when the counter wakes up
        base.OnEnable();
        order ??= new();
        order.Initialize();
    }

    protected override void Start()
    {   
        // build the view once the scene is ready
        base.Start();
        view ??= new(this.viewData);
        view.UpdateCounterTimer("--:--");
        view.UpdateScoreView(string.Empty , 0);
    }

#region order
    public Order GetOrder() => this.order;

    public void DispatchOrder(List<KitchenItem> orderItems)
    {   
        // clear the old order and start a fresh timer
        ClearOrder();
        order?.AddItems(orderItems);
        InitializeOrderView();
        secsElapsedFromOrder = 0f;
        orderTimerRoutine = StartCoroutine(OrderTimerRoutine()); 
    }

    void InitializeOrderView()
    {
        // refresh the counter visuals from the active order
        view?.RefreshOrderView(this.order);
    }

    public bool ValidateOrder(KitchenItem[] items)
    {   
        // make sure the served items match the order list
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
        // send the served order to the scoring flow
        EventManager.PreOrderServed.Invoke(new OrderServeData(this, this.order , this.secsElapsedFromOrder));
        ClearOrder();
    }

    public void ClearOrder()
    {   
        // stop the timer and hide the active order
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
        // track how long the order has been waiting
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
        // pause or resume the order timer
        isPaused = value;
    }


#endregion    

    public async void DisplayScore(int scoreAmt)
    {   
        string str = scoreAmt.ToString();

        this.view.UpdateScoreView(str , scoreAmt);

        if(isDisplayingScore)
        {
            return;
        }

        isDisplayingScore = true;

        await Task.Delay(Mathf.FloorToInt(this.ScoreDisplayDuration * 1000));
        
        this.view.UpdateScoreView(string.Empty , scoreAmt);
        isDisplayingScore = false;
    }
    


    protected override void OnDisable()
    {   
        // dispose the order when the counter goes away
        base.OnDisable();
        order?.Dispose();
    }
}
