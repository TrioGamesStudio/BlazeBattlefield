using System.Collections.Generic;
using UnityEngine;

public class Backpack : MonoBehaviour
{



    public static Backpack instance;
    [SerializeField] private List<ItemLocalData> itemVen = new();
    private void Awake()
    {
        instance = this;
    }

    public void AddItemToInventory(ItemLocalData itemData)
    {
        foreach(var item in itemVen)
        {
            if (item.IsCountZero())
            {
                Debug.Log("StackWith all over");
                break;
            }
            if (item.CanStackWith(itemData))
            {
                Debug.Log("Start StackWith");
                item.StackWith(itemData);
                //BackpackUI.instance.UpdateUI(item.ItemIdentifier, item);
            }
        }
        if (!itemData.IsCountZero())
        {
            Debug.Log("After stacking, also have remaining count to add backpack", gameObject);
            itemVen.Add(itemData);
            //BackpackUI.instance.AddItemUI(itemData);
        }
        else
        {
            Debug.Log("Add all item into backpack");
        }
        
    }
    public void DropAll(ItemLocalData itemData)
    {

    }
    public void Drop(ItemLocalData itemData)
    {
        itemVen.Remove(itemData);
        CreateItemInWorld(itemData);
    }

    private void CreateItemInWorld(ItemLocalData itemData)
    {
        ItemGeneratorManager.instance.CreateFromItemData(itemData);
    }
    
    public bool CanCollect()
    {
        return true;
    }

    public void DropItemAmount(ItemLocalData currentItem, int dropAmount)
    {
        int currentCount = currentItem.CurrentQuantity;
        string itemName = currentItem.ItemName;
        string indentifyID = currentItem.ItemIdentifier;
        ItemDataSO itemDataSo = currentItem.ItemData;
        // Vector3 position = PlayerController.LocalPlayer.transform.position;
        Vector3 position = NetworkPlayer.Local.transform.position;

        if (dropAmount == currentCount)
        {
            // 
            Debug.Log("with new item after, maybe it will have more information, prepare for it", gameObject);
            //BackpackUI.instance.RemoveItemUI(currentItem);
            itemVen.Remove(currentItem);
            CreateItemInWorld(currentItem);
        }
        else if(dropAmount < currentCount)
        {
            currentItem.ModifyQuantity(-dropAmount);
            Debug.Log($"Drop {itemName} {currentCount}");
            //BackpackUI.instance.UpdateUI(indentifyID, currentItem);
            // ItemGeneratorManager.instance.CreateItemInWorld(itemDataSo, position, dropAmount);
            currentItem.SetQuantity(dropAmount);
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
    Ammor,
    Helmet,
    Backpack
}

