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



    public void Add(ItemType itemType,Enum _enum, InventoryItem inventoryItem)
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
        foreach(var data in inventoryItem.customDatas)
        {
            Debug.Log($"Key {data.key} Value {data.value}");
        }
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


}
public class InventoryItem
{
    public ItemType ItemType;
    public Enum _SubItemEnum;
    public string displayName;
    public Sprite Icon;
    public int maxStack;
    public int amount;
    public CustomData[] customDatas;
    public Action OnUpdateData;
}

public enum HealingItemType
{
    None = 0,
    Bandage = 5,
    FirstAidKit = 10,
    Medkit = 15,
}
