using UnityEngine;
using UnityEngine.UI;
// displays one order item in the reflection view
public class OrderReflectionSlot : MonoBehaviour
{
    Image image;

    void Start()
    {
        // cache the image once the slot is alive
        image = this.GetComponent<Image>();
    }

    public void EnableSlot()
    {
        // make the slot visible
        this.gameObject.SetActive(true);
    }

    public void Initialize(KitchenItem orderItem)
    {
        // show the item sprite inside the slot
        image.sprite = orderItem.sprite;
    }

    public void DisableSlot()
    {
        // hide the slot when it is not needed
        this.gameObject.SetActive(false);
        
    }
}
