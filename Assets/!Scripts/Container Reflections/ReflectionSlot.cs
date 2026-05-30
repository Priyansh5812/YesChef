using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// handles one draggable reflection slot
public class ReflectionSlot : MonoBehaviour , IBeginDragHandler , IEndDragHandler, IDragHandler , IDropHandler
{   
    Canvas m_canvas;
    [SerializeField] Image image;
        
    IContainerReflectionSystem system;    

    ReflectionKitchenItem item;


    void Start()
    {   
        // cache the local canvas and system
        InitializeComponents();
    }

    void InitializeComponents()
    {   
        // resolve the canvas and container system
        if(m_canvas == null)
            m_canvas = this.GetComponentInParent<Canvas>();

        if(system == null)
            system = this.GetComponentInParent<IContainerReflectionSystem>();

    }

    public void InitializeSlot(KitchenItem item, int index)
    {   
        // load the item into this slot
        this.item = new(item, index);
        UpdateSlotView();
    }

    void UpdateSlotView()
    {   
        // update the icon and visibility
        image.sprite = this.item.item == null ? null : this.item.item.sprite;
        image.color = this.item.item == null ? Color.clear : Color.white;
    }


    #region DRAG - DROP
    public void OnBeginDrag(PointerEventData eventData)
    {   
        // start moving the item with the cursor
        this.image.rectTransform.anchoredPosition += eventData.delta / m_canvas.scaleFactor;
        m_canvas.sortingOrder++;
        this.image.rectTransform.SetParent(this.transform.parent);
        IContainerReflectionSystem.IsUnderDragOperation = true;
        IContainerReflectionSystem.ActiveTransferRequest = PrepareItemTransferRequest(); 
    }

    public void OnDrag(PointerEventData eventData)
    {
        // keep the item following the pointer
        this.image.rectTransform.anchoredPosition += eventData.delta / m_canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {   
        // snap the item back into place
        this.image.rectTransform.SetParent(this.transform);
        this.image.rectTransform.anchoredPosition = Vector2.zero;
        m_canvas.sortingOrder--;
        IContainerReflectionSystem.IsUnderDragOperation = false;
        IContainerReflectionSystem.ActiveTransferRequest = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        // try to swap items between containers
        Debug.LogWarning("Dropped on "+ eventData.pointerDrag.gameObject.name);
        
        var req = IContainerReflectionSystem.ActiveTransferRequest;

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
        // build the current transfer request
        return new KitchenItemTransferRequest(this.item.item , system.associatedContainer, this.item.originalArrayIndex);
    }

}
