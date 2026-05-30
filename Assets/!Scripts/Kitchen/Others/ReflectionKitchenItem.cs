// pairs an item with its original slot
public struct ReflectionKitchenItem
{
    public ReflectionKitchenItem(KitchenItem item , int ind)
    {
        // keeps the item and its source index together
        this.item = item;
        originalArrayIndex = ind;
    }
    public KitchenItem item;

    public int originalArrayIndex;

}
