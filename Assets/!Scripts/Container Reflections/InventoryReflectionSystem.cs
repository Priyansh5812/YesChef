using UnityEngine;
using TMPro;

// builds the inventory reflection view
public class InventoryReflectionSystem : MonoBehaviour, IContainerReflectionSystem
{   
    // title shown above the inventory slots
    [SerializeField] TextMeshProUGUI title;
    ReflectionSlot[] slots;

    public IContainer associatedContainer
    {
        // container currently shown in the inventory panel
        get; private set;
    }

    void Awake()
    {
        // cache the reflection slots once
        slots = this.GetComponentsInChildren<ReflectionSlot>();
    }

    public void ReflectContainer(KitchenItem[] items, IContainer container)
    {
        // mirror the current container contents
        associatedContainer = container;
        associatedContainer.GetConfigInfo(out string title , out var funcType);

        foreach(var i in slots)
            i.gameObject.SetActive(false);

         if(items.Length > slots.Length)
        {
            Debug.LogError($"Item count exceeds the cached slot length\n items: {items.Length}\n slots: {slots.Length}");
            return;
        }

        int size = items.Length;

        for(int i = 0 ; i < size; i++)
        {
            slots[i].gameObject.SetActive(true);
            slots[i].InitializeSlot(items[i] , i);
        }

        SetupTitle(title);
    }

    void SetupTitle(string title)
    {
        // update the panel title
        this.title?.SetText(title);
    }
}
