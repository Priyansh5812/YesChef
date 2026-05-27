using UnityEngine;
public class ContainerReflectionSystem : MonoBehaviour
{   
    ReflectionSlot[] slots;
    public IContainer associatedContainer
    {
        get; private set;
    }

    public static KitchenItemTransferRequest? ActiveTransferRequest
    {
        get; set;
    } = null;

    void Awake()
    {
        slots = this.GetComponentsInChildren<ReflectionSlot>();
    }

    public void ReflectContainer(KitchenItem[] items , IContainer container)
    {   
        associatedContainer = container;

        foreach(var i in slots)
            i.gameObject.SetActive(false);

        if(items.Length > slots.Length)
        {
            Debug.LogError("Item count exceeds the cached slot length");
            return;
        }

        int size = items.Length;

        for(int i = 0 ; i < size; i++)
        {
            slots[i].gameObject.SetActive(true);
            slots[i].InitializeSlot(items[i] , i);
        }
    }
}
