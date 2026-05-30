using UnityEngine.Pool;
using UnityEngine.UI;
using System.Collections.Generic;
// stores the list of items that make up one order
public class Order : System.IDisposable
{   
    List<KitchenItem> orderItems;   
    public int Count
    {
        get => orderItems == null ? 0 : orderItems.Count;
    }
    
    public void Initialize()
    {
        // grab a pooled list for the current order
        this.orderItems ??= ListPool<KitchenItem>.Get();
    }

    public void AddItems(List<KitchenItem> items)
    {
        // copy the order items into the active list
        orderItems.AddRange(items);
    }

    public void ClearOrderItems()
    {
        // remove all items without releasing the pool
        orderItems.Clear();
    }

    public KitchenItem GetOrderItemAt(int targetIndex) => orderItems.Count <= targetIndex ? null : orderItems[targetIndex];

    public bool IsOrderItemEqual(KitchenItem item, int targetIndex)
    {   
        // compare one order slot against the provided item
        if(item == null)
            return false;

        if(orderItems.Count <= targetIndex)
            return false;

        return orderItems[targetIndex].IsEqual(item);
    } 

    public void GetOrderItems(List<KitchenItem> ls)
    {
        // copy the order into another list
        ls.AddRange(orderItems);
    }
    

    public void Dispose()
    {
        // return the pooled list when the order is done
        if(this.orderItems != null)
        {
            ListPool<KitchenItem>.Release(this.orderItems);
            this.orderItems = null;
        }
    }
}
