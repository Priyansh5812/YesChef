public struct OrderServeData
{   
    public OrderServeData(CounterInteractable counter, Order order , float elapsedTime)
    {   this.counter = counter;
        this.order = order;
        this.timeElapsed = elapsedTime;
    }
    public CounterInteractable counter;
    public Order order;
    public float timeElapsed;
}