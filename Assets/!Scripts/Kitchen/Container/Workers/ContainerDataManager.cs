using UnityEngine;
// owns the raw item data for a container
public class ContainerDataManager
{
    public KitchenItem[] ContainerData
    {get; private set;}
    IContainer container;
    ContainerConfig m_data;
    

    public ContainerDataManager(IContainer container, ContainerConfig config)
    {
        // remember the container config and prepare the slots
        this.container = container;
        this.m_data = config;
        Initialize();
    }

    void Initialize()
    {   
        // build the slot array from the config
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
        // place an item into an empty slot
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
        // clear a slot and restock if the config allows it
        if(ContainerData[targetIndex] == null)
            return false;

        ContainerData[targetIndex] = null;
        
        if(m_data.CanRestockFromConfig)
            Restock(targetIndex);

        return true;
    }

    void Restock(int restockIndex)
    {
        // rebuild the slot item from the config template
        ContainerData[restockIndex] = new KitchenItem(m_data.InitItems[restockIndex]);
    }

    public bool EvaluateItemAddition(KitchenItem item, int targetIndex)
    {   
        // check whether the item can be added to this slot
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
        // check whether the slot can be emptied
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
        // scan the slots for any remaining items
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
