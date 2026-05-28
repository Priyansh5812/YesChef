using UnityEngine.Pool;
using System.Collections.Generic;
public class Order : System.IDisposable
{   
    public List<KitchenItem> orderItems;   

    public void Initialize()
    {
        this.orderItems ??= ListPool<KitchenItem>.Get();
    }

    public void AddOrderItem(KitchenItem item)
    {
        orderItems.Add(item);
    }

    public void ClearOrderItems()
    {
        orderItems.Clear();
    }

    public void Dispose()
    {
        if(this.orderItems != null)
        {
            ListPool<KitchenItem>.Release(this.orderItems);
            this.orderItems = null;
        }
    }
}
