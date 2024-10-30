using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
public class BackpackUI : MonoBehaviour
{
    public static BackpackUI instance;
    protected UnityPool<ItemBackpackUI> poolItemsUI;
    [SerializeField] protected ItemBackpackUI itemCollectUIPrefab;
    [SerializeField] protected GameObject content;
    [SerializeField] protected ItemDefaultConfigs itemDefaultConfig;
    [SerializeField] protected BackpackButtonGroupUI buttonGroupUI;
    [SerializeField] protected DropAmountUI dropAmountUI;
    [SerializeField] protected int dropCount;
    private InventoryItem currentItem;

    private Dictionary<ItemType, List<ItemBackpackUI>> _itemUIs = new();

    public void Setup()
    {
        instance = this;
        itemCollectUIPrefab.gameObject.SetActive(false);
        poolItemsUI = new UnityPool<ItemBackpackUI>(itemCollectUIPrefab, 10, content.transform);
        HideButton();
        StorageManager.OnAddItem += HandleItemAdded;
        StorageManager.OnRemoveItem += HandleItemRemove;


        buttonGroupUI.SetOnDropFull(DropAllItem);
        buttonGroupUI.SetOndropItemAmount(ShowDropAmount);

        dropAmountUI.SetAcceptDrop(OnAcceptDrop);
    }


    private void OnDestroy()
    {
        StorageManager.OnAddItem -= HandleItemAdded;
        StorageManager.OnRemoveItem -= HandleItemRemove;

        buttonGroupUI.SetOnDropFull(null);
        buttonGroupUI.SetOndropItemAmount(null);

        dropAmountUI.SetAcceptDrop(null);

        
    }


    public void ShowDropAmount()
    {
        dropAmountUI.Show();
        dropAmountUI.SetupView(currentItem);
    }

    private void OnAcceptDrop(int newDropCount)
    {
        if (newDropCount == 0) return;

        dropCount = newDropCount;
        if(dropCount == currentItem.amount)
        {
            DropAllItem();
        }
        else
        {
            ItemDatabase.instance.InventoryItemToWorld(currentItem, newDropCount);
            currentItem.amount -= newDropCount;
            currentItem?.OnUpdateData();
        }
        HideDropAmount();
        HideButton();
        Debug.Log("Drop With Count:" + newDropCount);
    }

    private void DropAllItem()
    {
        HideButton();
        HideDropAmount();
        ItemDatabase.instance.InventoryItemToWorld(currentItem, currentItem.amount);
        StorageManager.instance.Remove(currentItem.ItemType, currentItem._SubItemEnum, currentItem);
    }


    private void HandleItemAdded(InventoryItem item)
    {
        if(item == null)
        {
            Debug.Log("This item is null", gameObject);
            return;
        }

        var ui = poolItemsUI.Get();
        ui.Initialize(item);

        if (!_itemUIs.ContainsKey(item.ItemType))
            _itemUIs[item.ItemType] = new List<ItemBackpackUI>();
       
        _itemUIs[item.ItemType].Add(ui);
        ui.gameObject.SetActive(true);
    }

    private void HandleItemRemove(InventoryItem item)
    {
        if (_itemUIs.TryGetValue(item.ItemType, out var uis) && uis.Count > 0)
        {
            foreach (var itemUI in uis)
            {
                if (itemUI.AreSameItem(item))
                {
                    var key = item.ItemType;
                    uis.Remove(itemUI);
                    itemUI.OnRelease();
                    break;
                }
            }
        }

    }



    public void HideDropAmount()
    {
        dropAmountUI.gameObject.SetActive(false);
    }

    public void ShowButton(int transformIndex)
    {
        buttonGroupUI.gameObject.SetActive(true);
        buttonGroupUI.transform.SetSiblingIndex(transformIndex);
    }

    public void HideButton()
    {
        buttonGroupUI.gameObject.SetActive(false);
    }

    public void SetCurrentItem(InventoryItem inventoryItem)
    {
        currentItem = inventoryItem;
    }

}
