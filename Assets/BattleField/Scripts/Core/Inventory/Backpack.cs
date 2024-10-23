using System.Collections.Generic;
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
        foreach(var item in itemVen)
        {
            if (item.IsCountZero())
            {
                Debug.Log("Stack all over");
                break;
            }
            if (item.CanStack(itemData))
            {
                Debug.Log("Start Stack");
                item.Stack(itemData);
                BackpackUI.instance.UpdateUI(item.GetIndentifyID(), item);
            }
        }
        if (!itemData.IsCountZero())
        {
            Debug.Log("After stacking, also have remaining count to add backpack", gameObject);
            itemVen.Add(itemData);
            BackpackUI.instance.AddItemUI(itemData);
        }
        else
        {
            Debug.Log("Add all item into backpack");
        }
        
    }
    public void DropAll(ItemData itemData)
    {

    }
    public void Drop(ItemData itemData)
    {
        itemVen.Remove(itemData);
        CreateItemInWorld(itemData);
    }

    private void CreateItemInWorld(ItemData itemData)
    {
        ItemGeneratorManager.instance.CreateFromItemData(itemData);
    }
    
    public bool CanCollect()
    {
        return true;
    }

    public void DropItemAmount(ItemData currentItem, int dropAmount)
    {
        int currentCount = currentItem.GetCount();
        string itemName = currentItem.GetItemName();
        string indentifyID = currentItem.GetIndentifyID();
        ItemDataSO itemDataSo = currentItem.GetItemDataSO();
        Vector3 position = PlayerController.LocalPlayer.GetSoilderPosition();
        if (dropAmount == currentCount)
        {
            // 
            Debug.Log("with new item after, maybe it will have more information, prepare for it", gameObject);
            BackpackUI.instance.RemoveItemUI(currentItem);
            itemVen.Remove(currentItem);
            CreateItemInWorld(currentItem);
        }
        else if(dropAmount < currentCount)
        {
            currentItem.Decrease(dropAmount);
            Debug.Log($"Drop {itemName} {currentCount}");
            BackpackUI.instance.UpdateUI(indentifyID, currentItem);
            // ItemGeneratorManager.instance.CreateItemInWorld(itemDataSo, position, dropAmount);
            currentItem.SetCount(dropAmount);
            CreateItemInWorld(currentItem);

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

