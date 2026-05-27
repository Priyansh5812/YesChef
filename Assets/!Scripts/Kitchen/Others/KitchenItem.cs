using UnityEngine;

[System.Serializable]
public class KitchenItem
{
    public KitchenItemType itemType;
    public Sprite sprite;
     public bool isChopped;
     public bool isCooked;

    public KitchenItem(KitchenItem otherItem)
    {
        this.itemType = otherItem.itemType;
        this.sprite = otherItem.sprite;
        this.isChopped = otherItem.isChopped;
        this.isCooked = otherItem.isCooked;
    }
}
