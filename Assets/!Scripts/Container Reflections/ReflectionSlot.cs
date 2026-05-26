using UnityEngine;
using UnityEngine.UI;

public class ReflectionSlot : MonoBehaviour
{
    [SerializeField] Image image;
    
    ReflectionKitchenItem item;

    public void Initialize(KitchenItem item, int index)
    {   
        this.item = new(item, index);
        
        UpdateSlotView();
    }

    void UpdateSlotView()
    {
        image.sprite = this.item.item.sprite;
    }
    
}