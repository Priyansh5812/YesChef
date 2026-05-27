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
    Coroutine ProgressionTickRoutine = null;

    ReflectionSlot[] slots;
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
    }

    void OnEnable()
    {
        actionBtn?.onClick.AddListener(PerformContainerAction);

        if(canTickProgression)
            ProgressionTickRoutine = StartCoroutine(ReflectContainerAssociatedActiveFunctions());
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



    #region Container associated active function reflection

    bool isVisible = true;

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

    void OnDisable()
    {
        actionBtn?.onClick.RemoveListener(PerformContainerAction);
        if(ProgressionTickRoutine != null)
        {
            StopCoroutine(ProgressionTickRoutine);
        }
    }
}
