using UnityEngine;

[System.Serializable]
public class KitchenItem
{
    public KitchenItemType itemType;
    public Sprite sprite;
    public bool isChoppable;
    [HideInInspector] public bool isChopped;
    public bool isCookable;
    [HideInInspector] public bool isCooked;

    public KitchenItem(KitchenItem otherItem)
    {
        this.itemType = otherItem.itemType;
        this.sprite = otherItem.sprite;
        this.isChoppable = otherItem.isChoppable;
        this.isChopped = otherItem.isChopped;
        this.isCookable = otherItem.isCookable;
        this.isCooked = otherItem.isCooked;
    }
}
