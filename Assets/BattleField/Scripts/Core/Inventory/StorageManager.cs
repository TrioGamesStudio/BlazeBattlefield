using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
public class StorageManager : MonoBehaviour
{
    public static StorageManager instance;

    public static Action<InventoryItem> OnAddItem;
    public static Action<InventoryItem> OnRemoveItem;
    public static Action OnUpdateItem;

    private Dictionary<(ItemType, Enum), List<InventoryItem>> bigData = new();

    private void Awake()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        OnAddItem = null;
        OnRemoveItem = null;
        OnUpdateItem = null;
    }

    public void Add(ItemType itemType, Enum _enum, InventoryItem inventoryItem)
    {
        if (bigData.ContainsKey((itemType, _enum)) == false)
            bigData.Add((itemType, _enum), new List<InventoryItem>());

        if (bigData.TryGetValue((itemType, _enum), out var list))
        {
            foreach(var item in list)
            {
                if (inventoryItem.amount == 0)
                    break;

                bool itemHasMoreSpace = item.amount < item.maxStack;
                if (itemHasMoreSpace)
                {
                    
                    int totalAmount = item.amount + inventoryItem.amount;
                   
                    if (totalAmount <= item.maxStack) // Can add all item
                    {
                        item.amount += inventoryItem.amount;
                        inventoryItem.amount = 0;
                    }
                    else // need iterator more item // 7 + 5 = 12 => totalAmount - maxStack 
                    {
                        item.amount = item.maxStack;
                        inventoryItem.amount = totalAmount- item.maxStack;
                    }
                    // callback in UI
                    item.OnUpdateData();
                }
            }
            

            if (inventoryItem.amount == 0) return;
            // add new item to inventory
            list.Add(inventoryItem);
            OnAddItem(inventoryItem);
            ShowItemInformation(inventoryItem);
            OnUpdateItem?.Invoke();
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
            OnUpdateItem?.Invoke();
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

    public int GetTotalAmmo(AmmoType ammoType)
    {
        int totalAmmo = 0;
        if (bigData.TryGetValue((ItemType.Ammo, ammoType), out var list))
        {
            foreach(var item in list)
            {
                totalAmmo += item.amount;
            }
        }
        Debug.Log($"{ammoType.ToString()} : {totalAmmo}");
        return totalAmmo;
    }

    public int AcquireAmmoItem(AmmoType ammoType,int ammoNeed)
    {
        if (ammoNeed <= 0) return 0;

        int ammoCanGet = 0;
        if (!bigData.TryGetValue((ItemType.Ammo, ammoType), out var ammoList))
        {
            return 0;
        }
        
        var sortedAmmoList = ammoList.OrderBy(ammo => ammo.amount).ToList();
        ammoList = sortedAmmoList;
        
        List<InventoryItem> itemNeedUpdateUI = new();
        List<InventoryItem> removeItem = new();
        foreach (var ammo in ammoList) 
        {
            int ammoToTake = Math.Min(ammo.amount, ammoNeed - ammoCanGet);

            if (ammoToTake > 0)
            {
                ammo.amount -= ammoToTake;
                ammoCanGet += ammoToTake;

                if (ammo.amount <= 0)
                {
                    removeItem.Add(ammo);
                }
                else
                {
                    itemNeedUpdateUI.Add(ammo);
                }

                if (ammoCanGet >= ammoNeed)
                {
                    break;
                }
            }
        }

        foreach (var item in itemNeedUpdateUI)
            item.OnUpdateData();
        foreach(var item in removeItem)
        {
            Remove(ItemType.Ammo, ammoType, item);
        }
        return ammoCanGet;
    }
}

