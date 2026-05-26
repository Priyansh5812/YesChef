public struct ReflectionKitchenItem
{
    public ReflectionKitchenItem(KitchenItem item , int ind)
    {
        this.item = item;
        originalArrayIndex = ind;
    }
    public KitchenItem item;
    public int originalArrayIndex;

}