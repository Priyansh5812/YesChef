// packages a drag transfer between containers
public struct KitchenItemTransferRequest
{
    public KitchenItemTransferRequest(KitchenItem a , IContainer b , int c)
    {
        // keep the dragged item and its source together
        this.kitchenItem = a;
        this.associatedContainer = b;
        this.itemIndex = c;
    }

    public KitchenItem kitchenItem;
    public IContainer associatedContainer;
    public int itemIndex;
}
