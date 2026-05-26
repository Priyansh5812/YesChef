using UnityEngine;

public class ContainersReflectionView : MonoBehaviour
{
    [SerializeField] CanvasGroup cgMain;
    [SerializeField] ContainerReflectionSystem inventoryReflection;
    [SerializeField] ContainerReflectionSystem kitchenContainerReflection;

    void OnEnable()
    {
        InitListeners();
    }

    void InitListeners()
    {
        EventManager.GetInventoryReflectionReference.AddListener(this.GetInventoryReflectionSys);
        EventManager.GetKitchenContainerReflectionReference.AddListener(this.GetKitchenContainerReflectionSys);
        EventManager.OnContainerOpened.AddListener(OpenReflections);
        EventManager.OnContainerClosed.AddListener(CloseReflections);
    }

    void Start()
    {
        PrepareStartup();
    }

    void PrepareStartup()
    {
        CloseReflections();
    }

    public void CloseReflections()
    {
        cgMain.alpha = 0.0f;
        cgMain.interactable = cgMain.blocksRaycasts = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OpenReflections()
    {
        cgMain.alpha = 1.0f;
        cgMain.interactable = cgMain.blocksRaycasts = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void DeInitListeners()
    {
        EventManager.GetInventoryReflectionReference.RemoveListener(this.GetInventoryReflectionSys);
        EventManager.GetKitchenContainerReflectionReference.RemoveListener(this.GetKitchenContainerReflectionSys);
        EventManager.OnContainerOpened.RemoveListener(OpenReflections);
        EventManager.OnContainerClosed.RemoveListener(CloseReflections);
    }   

    void OnDisable()
    {
        DeInitListeners();   
    }

    #region OTHERS

    ContainerReflectionSystem GetInventoryReflectionSys() => this.inventoryReflection;
    ContainerReflectionSystem GetKitchenContainerReflectionSys() => this.kitchenContainerReflection;

    #endregion
}
