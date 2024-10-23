using System;
using UnityEngine;

[Serializable]
public class ItemLocalData
{
    [SerializeField]
    private readonly string itemIdentifier;
    [SerializeField] private int quantity;
    [SerializeField] private ItemDataSO ItemDataSO;
    // Constructor

    public string ItemIdentifier => itemIdentifier;
    public int CurrentQuantity => quantity;
    public string ItemName => ItemDataSO.ItemName;
    public ItemDataSO ItemData => ItemDataSO;
    public bool IsEmpty => quantity == 0;

    public ItemLocalData(ItemDataSO _ItemDataSO,int _count)
    {
        itemIdentifier = Guid.NewGuid().ToString();
        ItemDataSO = _ItemDataSO;
        quantity = ClampQuantity(_count);
    }
    private int ClampQuantity(int requestedQuantity)
    {
        return Math.Clamp(requestedQuantity, 0, ItemDataSO.maxCountPerStack);
    }


    public bool CanStackWith(ItemLocalData otherItem)
    {
        if (!AreItemEqualTypes(otherItem) || !ItemDataSO.canStackable)
            return false;

        return quantity < ItemDataSO.maxCountPerStack && otherItem.quantity > 0;
    }
    private bool AreItemEqualTypes(ItemLocalData otherItem)
    {
        return otherItem.ItemDataSO.ItemType == ItemDataSO.ItemType;
    }
    public void ModifyQuantity(int amount)
    {
        quantity = ClampQuantity(quantity + amount);
    }

    public void StackWith(ItemLocalData sourceItem)
    {
        int totalQuantity = quantity + sourceItem.quantity;

        if (WouldExceedMaxStack(totalQuantity))
        {
            HandlePartialStack(sourceItem, totalQuantity);
        }
        else
        {
            HandleFullStack(sourceItem);
        }
    }
    private bool WouldExceedMaxStack(int totalQuantity)
    {
        return totalQuantity > ItemDataSO.maxCountPerStack;
    }

    private void HandlePartialStack(ItemLocalData sourceItem, int totalQuantity)
    {
        quantity = ItemDataSO.maxCountPerStack;
        int remainingQuantity = totalQuantity - ItemDataSO.maxCountPerStack;
        sourceItem.SetQuantity(remainingQuantity);

    }

    private void HandleFullStack(ItemLocalData sourceItem)
    {
        quantity += sourceItem.quantity;
        sourceItem.SetQuantity(0);
    }
    public bool IsCountZero()
    {
        return quantity == 0;
    }

    public void SetQuantity(int newQuantity)
    {
        quantity = ClampQuantity(newQuantity);
    }
}