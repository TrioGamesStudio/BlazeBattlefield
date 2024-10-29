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
    private Dictionary<ItemType, List<ItemBackpackUI>> _itemUIs = new();
    private void Awake()
    {
        instance = this;
        itemCollectUIPrefab.gameObject.SetActive(false);
        poolItemsUI = new UnityPool<ItemBackpackUI>(itemCollectUIPrefab,10, content.transform);
        HideButton();
        StorageManager.OnAddItem = HandleItemAdded;
        StorageManager.OnRemoveItem = HandleItemRemove;


        buttonGroupUI.OnDropFullItem += DropItem;
        buttonGroupUI.OnDropItem += DropItem;

        dropAmountUI.OnDropItemCallback = OnDropAmountChange;
    }
    private void OnDestroy()
    {
        buttonGroupUI.OnDropFullItem -= DropItem;
        buttonGroupUI.OnDropItem -= DropItem;

        dropAmountUI.OnDropItemCallback = OnDropAmountChange;

    }



    private void OnDropAmountChange(int newDropCount)
    {
        dropCount = newDropCount;

        Debug.Log("Drop With Count:" + newDropCount);
    }

    private void HandleItemAdded(InventoryItem item)
    {
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
            foreach(var itemUI in uis)
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

    private InventoryItem currentItem;

    public void ShowDropAmount()
    {
        dropAmountUI.gameObject.SetActive(true);
        dropAmountUI.SetupView(currentItem);
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

    private void DropItem()
    {
        StorageManager.instance.Remove(currentItem.ItemType, currentItem._SubItemEnum, currentItem);
    }


    private ItemBackpackUI GetUIItem()
    {
        var itemBackpackUI = poolItemsUI.Get();
        return itemBackpackUI;
    }
    public HealingItemType HealingItemType;

    [Button]
    private void Test()
    {
        var itemData = itemDefaultConfig.FindItem(ItemType.Health,HealingItemType);
        if(itemData == null)
        {
            Debug.Log("Item Data is null");
        }
        else
        {
            itemData.ShowDebug();
            Debug.Log("Item Data not null");
        }
    }
}