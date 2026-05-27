using UnityEngine;

public class PlayerKitchenInteractor : MonoBehaviour
{   
    [SerializeField] ContainerConfig inventoryConfig;
    KitchenInteractable currentInteractable = null;
    SlotContainer inventoryContainer;
    ContainerReflectionSystem inventoryReflection;
    void OnEnable()
    {
        InitListeners();
    }

    void InitListeners()
    {
        EventManager.RefreshContainerReflections.AddListener(RefreshInventoryReflection);
    }

    void Start()
    {
        inventoryContainer = new SlotContainer(inventoryConfig);
        inventoryReflection = EventManager.GetInventoryReflectionReference.Invoke();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable?.TryInitiateInteraction(this.transform.forward);
        }    

    }

    void TryGetInteractor(Collider collider)
    { 
        if(collider.gameObject.TryGetComponent<KitchenInteractable>(out var comp))
        {   
            currentInteractable = comp;
        }
    }

    void TryReleaseInteractor(Collider collider)
    {
        if(collider.gameObject.TryGetComponent<KitchenInteractable>(out var comp))
        {   
            currentInteractable = currentInteractable.GetEntityId() == comp.GetEntityId() ? null : currentInteractable;
        }
    }


    void OnTriggerEnter(Collider collider)
    {   
        TryGetInteractor(collider);
    }

    void OnTriggerExit(Collider collider)
    {
        TryReleaseInteractor(collider);
        
    }

    #region INVENTORY

    void RefreshInventoryReflection()
    {
        this.inventoryContainer.UpdateReflection(inventoryReflection);
    }

    #endregion
    void DeInitListeners()
    {
        EventManager.RefreshContainerReflections.RemoveListener(RefreshInventoryReflection);
    }

    void OnDisable()
    {
        DeInitListeners();
    }

}
