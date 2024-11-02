using System;
using System.Collections.Generic;
using UnityEngine;
public class StorageManager : MonoBehaviour
{
    public static StorageManager instance;

    public static Action<InventoryItem> OnAddItem;
    public static Action<InventoryItem> OnRemoveItem;
    public static Action<InventoryItem> OnUpdateItem;

    private Dictionary<(ItemType, Enum), List<InventoryItem>> bigData = new();

    private void Awake()
    {
        instance = this;
    }

    public void Add(ItemType itemType, Enum _enum, InventoryItem inventoryItem)
    {
        if (bigData.ContainsKey((itemType, _enum)) == false)
            bigData.Add((itemType, _enum), new List<InventoryItem>());

        if (bigData.TryGetValue((itemType, _enum), out var list))
        {
            list.Add(inventoryItem);
            OnAddItem(inventoryItem);
            ShowItemInformation(inventoryItem);
        }
    }

    private void ShowItemInformation(InventoryItem inventoryItem)
    {
        Debug.Log($"name{inventoryItem.displayName},amoumt {inventoryItem.amount},item type {inventoryItem.ItemType},max stack {inventoryItem.maxStack}");
    }

    public void Remove(ItemType itemType, Enum _enum, InventoryItem inventoryItem)
    {
        if (bigData.TryGetValue((itemType, _enum), out var list))
        {
            if (!list.Contains(inventoryItem)) return;
            list.Remove(inventoryItem);
            OnRemoveItem(inventoryItem);
        }
    }

    public void DropAll(InventoryItem currentItem)
    {
        ItemDatabase.instance.InventoryItemToWorld(currentItem, currentItem.amount);
        Remove(currentItem.ItemType, currentItem._SubItemEnum, currentItem);
    }

    public void SplitItem(InventoryItem currentItem, int newDropCount)
    {
        ItemDatabase.instance.InventoryItemToWorld(currentItem, newDropCount);
        currentItem.amount -= newDropCount;
        currentItem?.OnUpdateData();
    }
}

