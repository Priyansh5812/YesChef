using UnityEngine.Pool;
using UnityEngine.UI;
using System.Collections.Generic;
public class Order : System.IDisposable
{   
    List<KitchenItem> orderItems;   
    public int Count
    {
        get => orderItems == null ? 0 : orderItems.Count;
    }
    
    public void Initialize()
    {
        this.orderItems ??= ListPool<KitchenItem>.Get();
    }

    public void AddItems(List<KitchenItem> items)
    {
        orderItems.AddRange(items);
    }

    public void ClearOrderItems()
    {
        orderItems.Clear();
    }

    public KitchenItem GetOrderItemAt(int targetIndex) => orderItems.Count <= targetIndex ? null : orderItems[targetIndex];

    public bool IsOrderItemEqual(KitchenItem item, int targetIndex)
    {   
        if(item == null)
            return false;

        if(orderItems.Count <= targetIndex)
            return false;

        return orderItems[targetIndex].IsEqual(item);
    } 

    public void GetOrderItems(List<KitchenItem> ls)
    {
        ls.AddRange(orderItems);
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
