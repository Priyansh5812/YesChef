using UnityEngine;


public class CounterInteractable : KitchenInteractable
{
    Order order;

    void OnEnable()
    {
        order ??= new();
        order.Initialize();
        
    }
    public Order GetOrder() => this.order;

    public void AddOrder(KitchenItem item)
    {   
        if(item == null)
        {
            Debug.LogError("A Null Item was intended to be registered as an order");
            return;
        }
        
        order?.AddOrderItem(item);
    }

    public void ClearOrder()
    {
        order?.ClearOrderItems();
    }

    void OnDisable()
    {
        order?.Dispose();
    }
}



public class CounterInteractableView
{
    
}
