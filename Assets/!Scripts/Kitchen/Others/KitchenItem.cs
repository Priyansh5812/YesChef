using UnityEngine;

[System.Serializable]
// stores one kitchen item and its cooked state
public class KitchenItem
{
    public KitchenItemType itemType;
    public Sprite sprite;
    public bool isChopped;
    public bool isCooked;

    public KitchenItem(KitchenItem otherItem)
    {
        // makes a copy of an existing item
        this.itemType = otherItem.itemType;
        this.sprite = otherItem.sprite;
        this.isChopped = otherItem.isChopped;
        this.isCooked = otherItem.isCooked;
    }

    public bool IsEqual(KitchenItem other)
    {
        // checks whether two items match
        return (
            this.itemType == other.itemType
            &&
            this.isChopped == other.isChopped
            &&
            this.isCooked == other.isCooked
        );
    }
}
