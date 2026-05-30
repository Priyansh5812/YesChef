using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class ContainerReflectionSystem : MonoBehaviour
{   
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI actionTitle;
    [SerializeField] Button actionBtn;
    [SerializeField] TextMeshProUGUI timeRemaining;
    [SerializeField] Slider loadingSlider;
    [SerializeField] bool canTickProgression;
    [SerializeField] GameObject OrdersParent;


    Coroutine ProgressionTickRoutine = null;

    ReflectionSlot[] slots;
    OrderReflectionSlot[] orderSlots;
    public IContainer associatedContainer
    {
        get; private set;
    }

    public static KitchenItemTransferRequest? ActiveTransferRequest
    {
        get; set;
    } = null;

    public static bool IsUnderDragOperation
    {
        get;  set;
    } = false;

    void Awake()
    {
        slots = this.GetComponentsInChildren<ReflectionSlot>();
        if(OrdersParent != null)
            orderSlots = this.OrdersParent.transform.GetComponentsInChildren<OrderReflectionSlot>();
    }

    void OnEnable()
    {
        InitListeners();

    }
    
    void InitListeners()
    {
        EventManager.OnGameOver.AddListener(StopProgressionTick);
        EventManager.OnGameStarted.AddListener(InitiateProgressionTick);
        actionBtn?.onClick.AddListener(PerformContainerAction);
    }




    public void ReflectContainer(KitchenItem[] items , IContainer container)
    {   
        associatedContainer = container;
        associatedContainer.GetConfigInfo(out string title , out var funcType);

        foreach(var i in slots)
            i.gameObject.SetActive(false);

        if(funcType == ContainerFunctionType.SERVE)
        {
            ReflectAsCounterContainer(items);
        }
        else
        {
            ReflectAsKitchenContainer(items);
        }

        SetupTitle(title);
        SetupFunctionAuthority(funcType);
    }


    void ReflectAsCounterContainer(KitchenItem[] items)
    {
        if(this.OrdersParent != null)
            this.OrdersParent?.SetActive(true);

        Order order = this.associatedContainer.GetCounterOrder();
        foreach(var i in orderSlots)
        {
            i.DisableSlot();
        }

        int orderItemsCount = order.Count;

        if(orderItemsCount > orderSlots.Length)
        {
            Debug.LogError($"Order item count exceeds the cached order slot length\n items: {orderItemsCount}\n slots: {orderSlots.Length}");
            return;
        }
        
        for(int i = 0 ; i < orderItemsCount; i++)
        {   
            orderSlots[i].EnableSlot();
            orderSlots[i].Initialize(order.GetOrderItemAt(i));
        }

        if(orderItemsCount > slots.Length)
        {
            Debug.LogError($"Order item count exceeds the cached slot length\n items: {orderItemsCount}\n slots: {slots.Length}");
            return;
        }

        for(int i = 0; i < orderItemsCount; i++)
        {
            slots[i].gameObject.SetActive(true);
            slots[i].InitializeSlot(items[i] , i);
        }


    }
    
    void ReflectAsKitchenContainer(KitchenItem[] items)
    {   
        if(this.OrdersParent != null)
            this.OrdersParent?.SetActive(false);
            
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
            this.actionBtn?.transform.parent.gameObject.SetActive(false);
            return;
        }
        this.actionBtn?.transform.parent.gameObject.SetActive(true);
        this.actionTitle?.SetText(functionType.ToString());
    }



    #region Container associated active function reflection

    bool isVisible = true;

    void InitiateProgressionTick()
    {   
        if(!canTickProgression)
            return;

        StopProgressionTick();
        if(canTickProgression)
            ProgressionTickRoutine = StartCoroutine(ReflectContainerAssociatedActiveFunctions());
    }

    IEnumerator ReflectContainerAssociatedActiveFunctions()
    {
        while(canTickProgression)
        {
            
            if(loadingSlider == null || timeRemaining == null || associatedContainer == null)
            {   
                HideProgressionPanel();
                yield return null;
                continue;
            }
                
            associatedContainer.GetFunctionCompletionStat(out float progress , out float completionTime);

            if(completionTime == -1)
            {   
                HideProgressionPanel();
                yield return null;
                continue;
            }

            DisplayProgressionPanel();

            timeRemaining?.SetText((completionTime - Time.time).ToString("N0")+"s");
            loadingSlider.value = progress;

            yield return null;
        }

        ProgressionTickRoutine = null;
    }

    void HideProgressionPanel()
    {   
        if(!isVisible)
            return;

        timeRemaining?.SetText("");
        loadingSlider.gameObject.SetActive(false);
        isVisible = false;
    }

    void DisplayProgressionPanel()
    {   
        if(isVisible)
            return;

        loadingSlider.gameObject.SetActive(true);
        isVisible = true;
    }

    
    #endregion

    void PerformContainerAction()
    {
        associatedContainer?.PerformAction();
    }

    void StopProgressionTick()
    {   
        if(ProgressionTickRoutine != null)
        {
            StopCoroutine(ProgressionTickRoutine);
            ProgressionTickRoutine = null;
        }
    }

    void DeInitListeners()
    {   
        EventManager.OnGameOver.RemoveListener(StopProgressionTick);
        EventManager.OnGameStarted.RemoveListener(InitiateProgressionTick);
        actionBtn?.onClick.RemoveListener(PerformContainerAction);
    }

    void OnDisable()
    {
        DeInitListeners();

    }
}
