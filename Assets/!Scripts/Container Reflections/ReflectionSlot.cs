using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ReflectionSlot : MonoBehaviour , IBeginDragHandler , IEndDragHandler, IDragHandler , IDropHandler
{   
    Canvas m_canvas;
    [SerializeField] Image image;
        
    ContainerReflectionSystem system;    

    ReflectionKitchenItem item;


    void Start()
    {   
        InitializeComponents();
    }

    void InitializeComponents()
    {   
        if(m_canvas == null)
            m_canvas = this.GetComponentInParent<Canvas>();

        if(system == null)
            system = this.GetComponentInParent<ContainerReflectionSystem>();

    }

    public void InitializeSlot(KitchenItem item, int index)
    {   
        this.item = new(item, index);
        UpdateSlotView();
    }

    void UpdateSlotView()
    {   
        image.sprite = this.item.item == null ? null : this.item.item.sprite;
        image.color = this.item.item == null ? Color.clear : Color.white;
    }


    #region DRAG - DROP
    public void OnBeginDrag(PointerEventData eventData)
    {   
        this.image.rectTransform.anchoredPosition += eventData.delta / m_canvas.scaleFactor;
        m_canvas.sortingOrder++;
        this.image.rectTransform.SetParent(this.transform.parent);
        ContainerReflectionSystem.IsUnderDragOperation = true;
        ContainerReflectionSystem.ActiveTransferRequest = PrepareItemTransferRequest(); 
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.image.rectTransform.anchoredPosition += eventData.delta / m_canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {   
        this.image.rectTransform.SetParent(this.transform);
        this.image.rectTransform.anchoredPosition = Vector2.zero;
        m_canvas.sortingOrder--;
        ContainerReflectionSystem.IsUnderDragOperation = false;
        ContainerReflectionSystem.ActiveTransferRequest = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.LogWarning("Dropped on "+ eventData.pointerDrag.gameObject.name);
        
        var req = ContainerReflectionSystem.ActiveTransferRequest;

        if(this.system.associatedContainer == req.Value.associatedContainer)
            return;

        IContainerRequest additionEvaluationReq = new ContainerEvaluateItemAddition(req.Value.kitchenItem , this.item.originalArrayIndex);
        IContainerRequest removalEvaluationReq = new ContainerEvaluateItemRemoval(req.Value.itemIndex);
        if(this.system.associatedContainer.ProcessContainerRequest(additionEvaluationReq) && req.Value.associatedContainer.ProcessContainerRequest(removalEvaluationReq))
        {
            this.system.associatedContainer.ProcessContainerRequest(new ContainerAddItem(req.Value.kitchenItem,this.item.originalArrayIndex));
            req.Value.associatedContainer.ProcessContainerRequest(new ContainerRemoveItem(req.Value.itemIndex));
            EventManager.RefreshContainerReflections.Invoke();
        }
    }
    #endregion
    
    KitchenItemTransferRequest PrepareItemTransferRequest()
    {
        return new KitchenItemTransferRequest(this.item.item , system.associatedContainer, this.item.originalArrayIndex);
    }

}