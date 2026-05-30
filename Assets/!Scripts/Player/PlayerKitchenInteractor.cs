using UnityEngine;

public class PlayerKitchenInteractor : MonoBehaviour
{   
    [SerializeField] ContainerConfig inventoryConfig;
    KitchenInteractable currentInteractable = null;
    SlotContainer inventoryContainer;
    ContainerReflectionSystem inventoryReflection;

    bool canInteract;

    void OnEnable()
    {
        InitListeners();
    }

    void InitListeners()
    {   
        EventManager.OnGameStarted.AddListener(EnableInteraction);
        EventManager.OnGamePaused.AddListener(DisableInteraction);
        EventManager.OnGameResumed.AddListener(EnableInteraction);
        EventManager.OnGameOver.AddListener(DisableInteraction);
        EventManager.RefreshContainerReflections.AddListener(RefreshInventoryReflection);
    }

    void Start()
    {
        inventoryContainer = new SlotContainer(inventoryConfig , null);
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
            currentInteractable.EnableInteraction();
        }
    }

    void TryReleaseInteractor(Collider collider)
    {
        if(collider.gameObject.TryGetComponent<KitchenInteractable>(out var comp))
        {   
            currentInteractable?.DisableInteraction();
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

    void EnableInteraction()
    {
        canInteract = true;
    }

    void DisableInteraction()
    {
        canInteract = false;
    }

    void DeInitListeners()
    {   
        EventManager.OnGameStarted.RemoveListener(EnableInteraction);
        EventManager.OnGamePaused.RemoveListener(DisableInteraction);
        EventManager.OnGameResumed.RemoveListener(EnableInteraction);
        EventManager.OnGameOver.RemoveListener(DisableInteraction);
        EventManager.RefreshContainerReflections.RemoveListener(RefreshInventoryReflection);
    }

    void OnDisable()
    {
        DeInitListeners();
    }

}
