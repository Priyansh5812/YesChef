using UnityEngine;

public class PlayerKitchenInteractor : MonoBehaviour
{   
    KitchenInteractable currentInteractable = null;
    SlotContainer inventoryContainer;
    ContainerReflectionSystem inventoryReflection;
    void OnEnable()
    {
        InitListeners();
    }

    void InitListeners()
    {
        EventManager.OnPreContainerOpened.AddListener(RefreshInventoryReflection);
    }

    void Start()
    {
        inventoryContainer = new SlotContainer(null,2);
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
        if(!inventoryContainer.IsOpened)
            inventoryContainer.OpenContainer(inventoryReflection);
        else
            inventoryContainer.CloseContainer(inventoryReflection);
    }

    #endregion
    void DeInitListeners()
    {
        EventManager.OnPreContainerOpened.RemoveListener(RefreshInventoryReflection);
    }

    void OnDisable()
    {
        DeInitListeners();
    }

}
