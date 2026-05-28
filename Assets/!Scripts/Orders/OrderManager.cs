using UnityEngine;

public class OrderManager : MonoBehaviour
{   
    [SerializeField] OrderConfig orderConfig;
    CounterInteractable[] counters;

    void OnEnable()
    {
        counters = this.GetComponentsInChildren<CounterInteractable>();
    }

    void Start()
    {
        SampleDummyOrders();
    }

    void SampleDummyOrders()
    {
        if(counters == null || counters.Length == 0)
        {
            Debug.LogError("No Counters found");
            return;
        }

        foreach(var c in counters)
        {   
            int maxOrderItemCount = Random.Range(2 , 4);
            for(int i = 0; i < maxOrderItemCount; i++)
            {
                c.AddOrder(GetRandomKitchenItem());
            }
            Debug.Log($"Total Orders for {c.gameObject.name} are {maxOrderItemCount}");
        }
    }
    
    KitchenItem GetRandomKitchenItem()
    {
        return orderConfig.possibleOrderItems[Random.Range(0 , orderConfig.possibleOrderItems.Length)];
    }

}
