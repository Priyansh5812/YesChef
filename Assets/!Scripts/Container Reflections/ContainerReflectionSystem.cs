using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ContainerReflectionSystem : MonoBehaviour
{   
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI actionTitle;
    [SerializeField] Button actionBtn;
     
    ReflectionSlot[] slots;
    public IContainer associatedContainer
    {
        get; private set;
    }

    public static KitchenItemTransferRequest? ActiveTransferRequest
    {
        get; set;
    } = null;

    void Awake()
    {
        slots = this.GetComponentsInChildren<ReflectionSlot>();
    }

    void OnEnable()
    {
        actionBtn?.onClick.AddListener(PerformContainerAction);
    }

    public void ReflectContainer(KitchenItem[] items , IContainer container)
    {   
        associatedContainer = container;

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

        associatedContainer.GetConfigInfo(out string title , out var funcType);
        SetupTitle(title);
        SetupFunctionAuthority(funcType);
    }

    void SetupTitle(string title)
    {
        this.title?.SetText(title);
    }

    void SetupFunctionAuthority(ContainerFunctionType functionType)
    {   
        if(this.actionBtn == null)
            return;

        if(functionType == ContainerFunctionType.NONE)
        {
            this.actionBtn.transform.parent.gameObject.SetActive(false);
            return;
        }
        this.actionBtn.transform.parent.gameObject.SetActive(true);
        this.actionTitle?.SetText(functionType.ToString());
    }


    void PerformContainerAction()
    {
        associatedContainer?.PerformAction();
    }

    void OnDisable()
    {
        actionBtn?.onClick.RemoveListener(PerformContainerAction);
    }
}
