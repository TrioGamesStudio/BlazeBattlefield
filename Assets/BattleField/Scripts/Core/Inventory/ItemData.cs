using System;
using UnityEngine;

[Serializable]
public class ItemData
{
    [SerializeField] private string indentifyID;
    [SerializeField] private int currentCount;
    [SerializeField] private ItemDataSO ItemDataSO;
    // Constructor
    public ItemData(ItemDataSO _ItemDataSO,int _count)
    {
        indentifyID = Guid.NewGuid().ToString();
        ItemDataSO = _ItemDataSO;
        currentCount = Math.Clamp(_count,0,ItemDataSO.maxCountPerStack);
    }
    public string GetIndentifyID()
    {
        return indentifyID;
    }
    public int GetCount() { return currentCount; }
    public string GetItemName()
    {
        return ItemDataSO.ItemName;
    }

    public ItemDataSO GetItemDataSO()
    {
        return ItemDataSO;
    }
    public bool CanStack(ItemData newItemData)
    {
        var equalDataType = newItemData.ItemDataSO.ItemType == ItemDataSO.ItemType;
        if (equalDataType == false || ItemDataSO.canStackable == false)
            return false;

        return currentCount < ItemDataSO.maxCountPerStack && newItemData.currentCount > 0;
    }
    public void Increase(int addCount)
    {
        currentCount += addCount;
    }
    public void Decrease(int dropAmount)
    {
        currentCount -= dropAmount;
    }

    public void Stack(ItemData itemData)
    {
        int totalCount = currentCount + itemData.currentCount;
        UnityEngine.Debug.Log("Stacking");
        if (totalCount > ItemDataSO.maxCountPerStack)
        {
            currentCount = ItemDataSO.maxCountPerStack;
            itemData.currentCount = totalCount - ItemDataSO.maxCountPerStack;
            UnityEngine.Debug.Log("Stack with remain count: " + itemData.currentCount);
        }
        else
        {
            UnityEngine.Debug.Log("Stacking All");
            currentCount += itemData.currentCount;
            itemData.currentCount = 0;
        }
    }

    public bool IsCountZero()
    {
        return currentCount == 0;
    }

    public void SetCount(int dropAmount)
    {
        currentCount = dropAmount;
    }
}