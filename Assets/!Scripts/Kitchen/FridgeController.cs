using UnityEngine;
public class FridgeController : KitchenInteractable 
{   
    [Header("Stats")]
    [SerializeField] ContainerInitializationData m_data;
    [SerializeField , Min(1)] int fallbackSlotSize;
    SlotContainer container;
    ContainerReflectionSystem reflection;

    
    protected override void Awake()
    {   
        base.Awake();
        container ??= new(m_data , fallbackSlotSize);   
    }

    void Start()
    {
        reflection = EventManager.GetKitchenContainerReflectionReference.Invoke();
        container.Initialize();
    }

    protected override void InitiateInteraction()
    {
        Debug.Log("Fridge Interaction");
        
        if(!container.IsOpened)
            container.OpenContainer(reflection);
        else
            container.CloseContainer(reflection);
    }

    
}
