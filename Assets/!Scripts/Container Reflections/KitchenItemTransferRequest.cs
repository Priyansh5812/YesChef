public struct KitchenItemTransferRequest
{
    public KitchenItemTransferRequest(KitchenItem a , IContainer b , int c)
    {
        this.kitchenItem = a;
        this.associatedContainer = b;
        this.itemIndex = c;
    }

    public KitchenItem kitchenItem;
    public IContainer associatedContainer;
    public int itemIndex;
}