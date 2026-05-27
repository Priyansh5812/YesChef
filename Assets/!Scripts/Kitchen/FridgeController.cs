using UnityEngine;
public class FridgeController : KitchenInteractable 
{   
    [Header("Stats")]
    [SerializeField] ContainerConfig m_data;
    SlotContainer container;
    ContainerReflectionSystem reflection;

    
    protected override void Awake()
    {   
        base.Awake();
        container ??= new(m_data);   
    }
    
    void OnEnable()
    {
        EventManager.RefreshContainerReflections.AddListener(RefreshContainerReflection);
    }

    void Start()
    {
        reflection = EventManager.GetKitchenContainerReflectionReference.Invoke();
    }

    protected override void InitiateInteraction()
    {
        Debug.Log("Fridge Interaction");
        
        if(!container.IsOpened)
        {
            EventManager.RefreshContainerReflections.Invoke();
            container.OpenContainer();
        }
        else
            container.CloseContainer();
    }


    void RefreshContainerReflection()
    {
        this.container.UpdateReflection(reflection);
    }

    void OnDisable()
    {
        EventManager.RefreshContainerReflections.RemoveListener(RefreshContainerReflection);
    }
}
