using UnityEngine;
public class ContainerReflectionSystem : MonoBehaviour
{   
    ReflectionSlot[] slots;

    void Awake()
    {
        slots = this.GetComponentsInChildren<ReflectionSlot>();
    }

    public void ReflectContainer(KitchenItem[] items)
    {
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
            slots[i].Initialize(items[i] , i);
        }
    }
}
