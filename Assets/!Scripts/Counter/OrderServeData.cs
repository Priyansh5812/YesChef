public struct OrderServeData
{   
    // packages the served order details for scoring
    public OrderServeData(CounterInteractable counter, Order order , float elapsedTime)
    {   this.counter = counter;
        this.order = order;
        this.timeElapsed = elapsedTime;
    }
    public CounterInteractable counter;
    public Order order;
    public float timeElapsed;
}
