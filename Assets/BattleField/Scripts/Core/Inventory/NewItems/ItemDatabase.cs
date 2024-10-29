using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : NetworkBehaviour
{
    public List<NetworkObject> gameObjects;
    public static ItemDatabase instance;
    private Dictionary<(ItemType, Enum), NetworkObject> bigData = new();

    private void Awake()
    {
        instance = this;
        Convert();
    }

    public NetworkObject GetItemPrefab(ItemType key1,Enum key2) 
    {
        if(bigData.TryGetValue((key1,key2),out var prefab))
        {
            return prefab;
        }
        Debug.LogError($"This item {key1.ToString()} {key2.ToString()} is null");
        return null;
    }

    public void InventoryItemToWorld(InventoryItem inventoryItem, int newAmount)
    {
        var key1 = inventoryItem.ItemType;
        var key2 = inventoryItem._SubItemEnum;

        CreateItemInWorld(newAmount, key1, key2);
    }

    private void CreateItemInWorld(int newAmount, ItemType key1, Enum key2)
    {
        var itemPrefab = GetItemPrefab(key1, key2);
        if (itemPrefab == null) return;

        var item = Runner.Spawn(itemPrefab, transform.position).GetComponent<ItemDataEnum>();
        item.SetQuantity(newAmount);
    }

    private void Convert()
    {
        foreach(var item in gameObjects)
        {
            var itemEnum = item.GetComponent<ItemDataEnum>();
            if (itemEnum == null)
                continue;
            var key1 = itemEnum.GetItemType();
            var key2 = itemEnum.GetSubItemType();
            if (!bigData.ContainsKey((key1, key2)))
            {
                bigData.Add((key1, key2), item);
            }
            else
            {
                bigData[(key1, key2)] = item;
            }
        }
    }
}
