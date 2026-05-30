// request types used to update container contents
public interface IContainerRequest
{}

// adds an item at a specific slot
public struct ContainerAddItem : IContainerRequest
{   
    public ContainerAddItem(KitchenItem item , int targetIndex)
    {
        this.item = item;
        this.targetIndex = targetIndex;
    }

    public KitchenItem item;
    public int targetIndex;
}

// removes an item from a specific slot
public struct ContainerRemoveItem : IContainerRequest
{   
    public ContainerRemoveItem(int targetIndex)
    {
        this.targetIndex = targetIndex;
    }
    public int targetIndex;
}

// checks whether an item can be added
public struct ContainerEvaluateItemAddition : IContainerRequest
{   
    public ContainerEvaluateItemAddition(KitchenItem item , int targetIndex)
    {
        this.item = item;
        this.targetIndex = targetIndex;
    }

    public KitchenItem item;
    public int targetIndex;
}

// checks whether an item can be removed
public struct ContainerEvaluateItemRemoval : IContainerRequest
{
    public ContainerEvaluateItemRemoval(int targetIndex)
    {
        this.targetIndex = targetIndex;
    }
    public int targetIndex;
}
