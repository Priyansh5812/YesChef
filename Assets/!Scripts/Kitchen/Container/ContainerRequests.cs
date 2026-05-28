public interface IContainerRequest
{}

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

public struct ContainerRemoveItem : IContainerRequest
{   
    public ContainerRemoveItem(int targetIndex)
    {
        this.targetIndex = targetIndex;
    }
    public int targetIndex;
}

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

public struct ContainerEvaluateItemRemoval : IContainerRequest
{
    public ContainerEvaluateItemRemoval(int targetIndex)
    {
        this.targetIndex = targetIndex;
    }
    public int targetIndex;
}