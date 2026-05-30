using UnityEngine;

// controls the shared reflection panels
public class ContainersReflectionView : MonoBehaviour
{
    // the reflection panels and their data views
    [SerializeField] CanvasGroup cgMain;
    [SerializeField] InventoryReflectionSystem inventoryReflection;
    [SerializeField] KitchenReflectionSystem kitchenContainerReflection;

    void OnEnable()
    {
        // subscribe to the ui events
        InitListeners();
    }

    void InitListeners()
    {
        // wire the reflection sources and visibility toggles
        EventManager.GetInventoryReflectionReference.AddListener(this.GetInventoryReflectionSys);
        EventManager.GetKitchenContainerReflectionReference.AddListener(this.GetKitchenContainerReflectionSys);
        EventManager.OnContainerOpened.AddListener(OpenReflections);
        EventManager.OnContainerClosed.AddListener(CloseReflections);
    }

    void Start()
    {
        // start hidden until a container opens
        PrepareStartup();
    }

    void PrepareStartup()
    {
        // keep the view closed on load
        CloseReflections();
    }

    public void CloseReflections()
    {
        // hide the reflection ui and lock the cursor
        cgMain.alpha = 0.0f;
        cgMain.interactable = cgMain.blocksRaycasts = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OpenReflections()
    {
        // show the reflection ui and free the cursor
        cgMain.alpha = 1.0f;
        cgMain.interactable = cgMain.blocksRaycasts = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void DeInitListeners()
    {
        // remove the shared listeners
        EventManager.GetInventoryReflectionReference.RemoveListener(this.GetInventoryReflectionSys);
        EventManager.GetKitchenContainerReflectionReference.RemoveListener(this.GetKitchenContainerReflectionSys);
        EventManager.OnContainerOpened.RemoveListener(OpenReflections);
        EventManager.OnContainerClosed.RemoveListener(CloseReflections);
    }   

    void OnDisable()
    {
        // clean up when the view shuts down
        DeInitListeners();   
    }

    #region OTHERS

    // give the event system the right reflection target
    IContainerReflectionSystem GetInventoryReflectionSys() => this.inventoryReflection;
    // give the event system the kitchen reflection target
    IContainerReflectionSystem GetKitchenContainerReflectionSys() => this.kitchenContainerReflection;

    #endregion
}
