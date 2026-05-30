using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// builds the kitchen reflection view
public class KitchenReflectionSystem : MonoBehaviour, ITickableReflectionSystem
{   
    // title and action controls for the active container
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI actionTitle;
    [SerializeField] Button actionBtn;
    [SerializeField] TextMeshProUGUI timeRemaining;
    [SerializeField] Slider loadingSlider;
    [SerializeField] GameObject OrdersParent;

    OrderReflectionSlot[] orderSlots;

    ReflectionSlot[] slots;

    public IContainer associatedContainer
    {
        // container currently shown in the kitchen panel
        get; private set;
    }

    Coroutine ProgressionTickRoutine;


    void Awake()
    {
        // cache the slot views once
        slots = this.GetComponentsInChildren<ReflectionSlot>();
        orderSlots = this.OrdersParent.transform.GetComponentsInChildren<OrderReflectionSlot>();
    }

    
    void OnEnable()
    {
        // hook up the shared listeners
        InitListeners();

    }
    
    void InitListeners()
    {
        // react to game flow and action button clicks
        EventManager.OnGameOver.AddListener(StopProgressionTick);
        EventManager.OnGameStarted.AddListener(InitiateProgressionTick);
        actionBtn?.onClick.AddListener(PerformContainerAction);
    }

    public void ReflectContainer(KitchenItem[] items, IContainer container)
    {
        // mirror the active container into the view
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
        SetupFunctionAuthority(funcType);
        SetupTitle(title);
    }

    void ReflectAsCounterContainer(KitchenItem[] items)
    {
        // show the current order and matching items
        this.OrdersParent.SetActive(true);

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
        // show a regular kitchen container
        this.OrdersParent.SetActive(false);
            
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


    void SetupTitle(string str)
    {
        // update the main label
        this.title?.SetText(str);        
    }

    public void SetupFunctionAuthority(ContainerFunctionType functionType)
    {
        // show the action button when the container supports one
        if(functionType == ContainerFunctionType.NONE)
        {
            this.actionBtn?.transform.parent.gameObject.SetActive(false);
            return;
        }
        this.actionBtn?.transform.parent.gameObject.SetActive(true);
        this.actionTitle?.SetText(functionType.ToString());
    }

    bool isVisible = true;

    void InitiateProgressionTick()
    {   
        // restart the action timer display
        StopProgressionTick();
        ProgressionTickRoutine = StartCoroutine(ReflectContainerAssociatedActiveFunctions());
    }

    IEnumerator ReflectContainerAssociatedActiveFunctions()
    {   
        // keep the progress ui in sync
        while(true)
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

    }

    void StopProgressionTick()
    {   
        // stop the timer ui routine
        if(ProgressionTickRoutine != null)
        {
            StopCoroutine(ProgressionTickRoutine);
            ProgressionTickRoutine = null;
        }
    }


    void HideProgressionPanel()
    {   
        // hide the progress display when idle
        if(!isVisible)
            return;

        timeRemaining?.SetText("");
        loadingSlider.gameObject.SetActive(false);
        isVisible = false;
    }

    void DisplayProgressionPanel()
    {   
        // show the progress display when active
        if(isVisible)
            return;

        loadingSlider.gameObject.SetActive(true);
        isVisible = true;
    }

    void PerformContainerAction()
    {
        // ask the container to run its action
        associatedContainer?.PerformAction();
    }

    void DeInitListeners()
    {   
        // remove the shared hooks
        EventManager.OnGameOver.RemoveListener(StopProgressionTick);
        EventManager.OnGameStarted.RemoveListener(InitiateProgressionTick);
        actionBtn?.onClick.RemoveListener(PerformContainerAction);
    }

    void OnDisable()
    {
        // clean up when the view is disabled
        DeInitListeners();
    }

}
