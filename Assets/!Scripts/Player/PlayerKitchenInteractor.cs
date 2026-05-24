using UnityEngine;

public class PlayerKitchenInteractor : MonoBehaviour
{   
    KitchenInteractable currentInteractable = null;

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

}
