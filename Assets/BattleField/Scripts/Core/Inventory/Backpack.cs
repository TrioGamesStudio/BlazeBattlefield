using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using NaughtyAttributes;
using UnityEngine;

public class Backpack : MonoBehaviour
{
    public static Backpack instance;
    [SerializeField] private List<ItemData> itemVen = new();
    private void Awake()
    {
        instance = this;
    }

    public void AddItemToInventory(ItemData itemData)
    {
        itemVen.Add(itemData);
        BackpackUI.instance.AddItemUI(itemData);
    }
    public void DropAll(ItemData itemData)
    {

    }
    public void Drop(ItemData itemData)
    {
        itemVen.Remove(itemData);
        ItemGeneratorManager.instance.CreateItemInWorld(itemData.ItemDataSO,itemData.count);

    }
    public bool CanCollect()
    {
        return true;
    }

    public void DropItemAmount(ItemData currentItem, int dropAmount)
    {
        if(dropAmount == currentItem.count)
        {
            // 
            Debug.Log("with new item after, maybe it will have more information, prepare for it", gameObject);
            BackpackUI.instance.RemoveItemUI(currentItem);
            ItemGeneratorManager.instance.CreateItemInWorld(currentItem.ItemDataSO,currentItem.count);
        }
        else if(dropAmount < currentItem.count)
        {
            currentItem.count -= dropAmount;
            Debug.Log($"Drop {currentItem.ItemDataSO.name} {currentItem.count}");
            BackpackUI.instance.UpdateUI(currentItem.indentifyID, currentItem);
            ItemGeneratorManager.instance.CreateItemInWorld(currentItem.ItemDataSO, dropAmount);

        }
    }
}


public enum ItemType
{
    Gun,
    Melee,
    Ammo,
    Health,
    Grenade,
}

