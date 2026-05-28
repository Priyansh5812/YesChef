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

    public bool IsEqual(KitchenItem other)
    {
        return (
            this.itemType == other.itemType
            &&
            this.isChopped == other.isChopped
            &&
            this.isCooked == other.isCooked
        );
    }
}
