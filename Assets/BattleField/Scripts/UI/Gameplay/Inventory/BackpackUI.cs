using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
public class BackpackUI : MonoBehaviour
{
    public static BackpackUI instance;
    protected UnityPool<ItemCollectUI> poolItemsUI;
    [SerializeField] protected ItemBackpackUI itemCollectUIPrefab;
    [SerializeField] protected GameObject content;
    [SerializeField] protected HealthItemData healthItemData;
    [SerializeField] protected BackpackButtonGroupUI buttonGroupUI;
    [SerializeField] protected DropAmountUI dropAmountUI;
    [SerializeField] protected int dropCount;
    private void Awake()
    {
        itemCollectUIPrefab.gameObject.SetActive(false);
        poolItemsUI = new UnityPool<ItemCollectUI>(itemCollectUIPrefab,10, content.transform);
    }

    public void UpdateHealthUI(Dictionary<HealingItemType,int> itemList)
    {
        foreach(var healthItem in itemList)
        {
            var itemConfig = healthItemData.GetItemDataConfig(healthItem.Key);
            int maxQuantityOfStack = itemConfig.maxStack;
            var itemBackpackUI = GetUIItem();
            itemBackpackUI.SetItemCount(healthItem.Value);
            itemBackpackUI.SetItemName(itemConfig.displayName);

            itemBackpackUI.SetItemBPData(itemConfig.ItemType, healthItem.Value, (int)healthItem.Key);
            itemBackpackUI.SetOnClickEvent(() =>
            {
                SetCurrentItem(itemBackpackUI);
                buttonGroupUI.ShowByIndex(itemBackpackUI.transform.GetSiblingIndex());
            });
        }
    }
    ItemBackpackUI.ItemBPData ItemBPData;
    public void SetCurrentItem(ItemBackpackUI itemBackpackUI)
    {
        ItemBPData = itemBackpackUI._ItemBPData;
    }
    private void OnDrop(ItemType itemType, int enumIndex, int quantity)
    {
        if(itemType == ItemType.Health)
        {
            StorageManager.instance.UpdateHealth((HealingItemType)enumIndex, quantity, false);
        }
        else if(itemType == ItemType.Ammo)
        {
            StorageManager.instance.UpdateAmmo((AmmoType)enumIndex, quantity, false);
        }
    }
    private ItemBackpackUI GetUIItem()
    {
        var itemBackpackUI = poolItemsUI.Get();
        return itemBackpackUI as ItemBackpackUI;
    }
    public HealingItemType HealingItemType;

    [Button]
    private void Test()
    {
        var itemData = healthItemData.GetItemDataConfig(HealingItemType);
        if(itemData == null)
        {
            Debug.Log("Item Data is null");
        }
        else
        {
            Debug.Log("Item Data not null");
        }
    }
}