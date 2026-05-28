using UnityEngine;
public class ContainerDataManager
{
    public KitchenItem[] ContainerData
    {get; private set;}
    IContainer container;
    ContainerConfig m_data;
    

    public ContainerDataManager(IContainer container, ContainerConfig config)
    {
        this.container = container;
        this.m_data = config;
        Initialize();
    }

    void Initialize()
    {   
        if(m_data.InitItems.Count == 0)
        {
            ContainerData = new KitchenItem[m_data.FallbackSlotCount];
            return;
        }


       ContainerData = new KitchenItem[m_data.InitItems.Count];
       for(int i = 0 ; i < ContainerData.Length; i++)
        {
            this.Restock(i);
        }
    }

    
    public bool AddItem(KitchenItem item, int targetIndex)
    {   
        Debug.Log("Before Add:");

        foreach(var i in ContainerData)
        {
            Debug.Log(i == null ? "NULL" : i.itemType);
        }

        if(ContainerData[targetIndex] == null)
        {
            this.ContainerData[targetIndex] = item;
            Debug.Log("After Add:");
            foreach(var i in ContainerData)
            {
                Debug.Log(i == null ? "NULL" : i.itemType);
            }
            return true;
        }
        else
            return false;
        
    }

    public bool RemoveItem(int targetIndex)
    {   
        if(ContainerData[targetIndex] == null)
            return false;

        ContainerData[targetIndex] = null;
        
        if(m_data.CanRestockFromConfig)
            Restock(targetIndex);

        return true;
    }

    void Restock(int restockIndex)
    {
        ContainerData[restockIndex] = new KitchenItem(m_data.InitItems[restockIndex]);
    }

    public bool EvaluateItemAddition(KitchenItem item, int targetIndex)
    {   
        if(container.IsContainerLocked)
            return false;

        if(!m_data.ItemAdditionCompatible)
            return false;

        if(item == null)
            return false;

        if(targetIndex >= ContainerData.Length)
            return false;
            
        if(m_data.CheckItemForOrderEligibility)
        {   
            var interactable = ((SlotContainer)this.container).GetAssociatedInteractable();

            if(interactable != null)
            {
                Order order = ((CounterInteractable) (interactable)).GetOrder();

                if(order.Count <= targetIndex)
                    return false;
                
                if(!order.IsOrderItemEqual(item , targetIndex))
                    return false;
            }
            else
            {
                Debug.LogWarning("Item Addition check was done under order with Null Interactable reference.\n Check was Skipped !!!");
            }

        }

        foreach(var i in m_data.IgnoreItemAdditionTypes)
        {
            if(i == item.itemType)
                return false;
        }

        return true;
    }

    public bool EvaluateItemRemoval(int targetIndex)
    {   
        if(container.IsContainerLocked)
            return false;

        if(!m_data.ItemRemovalCompatible)
            return false;

        if(targetIndex >= ContainerData.Length)
            return false;

        return true;
    }
    
    public bool IsContainerEmpty()
    {
        bool isContainerEmpty = true;
        foreach(var i in ContainerData)
        {
            if(i != null)
            {
                isContainerEmpty = false;
                break;
            }
        }

        return isContainerEmpty;
    }

}