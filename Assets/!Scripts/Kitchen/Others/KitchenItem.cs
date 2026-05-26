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
}
