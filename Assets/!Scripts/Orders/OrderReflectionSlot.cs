using UnityEngine;
using UnityEngine.UI;
public class OrderReflectionSlot : MonoBehaviour
{
    Image image;

    void Start()
    {
        image = this.GetComponent<Image>();
    }

    public void EnableSlot()
    {
        this.gameObject.SetActive(true);
    }

    public void Initialize(KitchenItem orderItem)
    {
        image.sprite = orderItem.sprite;
    }

    public void DisableSlot()
    {
        this.gameObject.SetActive(false);
        
    }
}
