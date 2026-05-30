using UnityEngine;

// handles player interaction with kitchen objects
public class PlayerKitchenInteractor : MonoBehaviour
{   
    [SerializeField] ContainerConfig inventoryConfig;
    KitchenInteractable currentInteractable = null;
    SlotContainer inventoryContainer;
    ContainerReflectionSystem inventoryReflection;

    // tracks whether the player may interact
    bool canInteract;

    void OnEnable()
    {
        // listen for game flow changes and reflection refreshes
        InitListeners();
    }

    void InitListeners()
    {   
        // keep the inventory in step with the game state
        EventManager.OnGameStarted.AddListener(EnableInteraction);
        EventManager.OnGamePaused.AddListener(DisableInteraction);
        EventManager.OnGameResumed.AddListener(EnableInteraction);
        EventManager.OnGameOver.AddListener(DisableInteraction);
        EventManager.RefreshContainerReflections.AddListener(RefreshInventoryReflection);
    }

    void Start()
    {
        // create the inventory container and its reflection target
        inventoryContainer = new SlotContainer(inventoryConfig , null);
        inventoryReflection = EventManager.GetInventoryReflectionReference.Invoke();
    }

    void Update()
    {
        // try the active interactable when the use key is pressed
        if(Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable?.TryInitiateInteraction(this.transform.forward);
        }    

    }

    void TryGetInteractor(Collider collider)
    { 
        // remember the interactable that the player just touched
        if(collider.gameObject.TryGetComponent<KitchenInteractable>(out var comp))
        {   
            currentInteractable = comp;
            currentInteractable.EnableInteraction();
        }
    }

    void TryReleaseInteractor(Collider collider)
    {
        // clear the current interactable when leaving its trigger
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
        // redraw the inventory reflection
        this.inventoryContainer.UpdateReflection(inventoryReflection);
    }

    #endregion

    void EnableInteraction()
    {
        // allow the player to interact again
        canInteract = true;
    }

    void DisableInteraction()
    {
        // block interaction while the game flow demands it
        canInteract = false;
    }

    void DeInitListeners()
    {   
        // remove all shared listeners before shutdown
        EventManager.OnGameStarted.RemoveListener(EnableInteraction);
        EventManager.OnGamePaused.RemoveListener(DisableInteraction);
        EventManager.OnGameResumed.RemoveListener(EnableInteraction);
        EventManager.OnGameOver.RemoveListener(DisableInteraction);
        EventManager.RefreshContainerReflections.RemoveListener(RefreshInventoryReflection);
    }

    void OnDisable()
    {
        // clean up listeners when the component is disabled
        DeInitListeners();
    }

}
