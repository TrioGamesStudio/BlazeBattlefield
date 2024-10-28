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
    [SerializeField] protected ItemDefaultConfigs itemDefaultConfig;
    [SerializeField] protected BackpackButtonGroupUI buttonGroupUI;
    [SerializeField] protected DropAmountUI dropAmountUI;
    [SerializeField] protected int dropCount;
    private void Awake()
    {
        itemCollectUIPrefab.gameObject.SetActive(false);
        poolItemsUI = new UnityPool<ItemCollectUI>(itemCollectUIPrefab,10, content.transform);
    }

    public void UpdateHealthUI(ItemType itemType,Dictionary<Enum,int> storage)
    {
        foreach(var healthItem in storage)
        {
            var itemConfig = itemDefaultConfig.FindItem(itemType, healthItem.Key);
            if(itemConfig == null)
            {
                Debug.LogError("Item Config is null, please check it ", gameObject);
                continue;
            }
            int maxQuantityOfStack = itemConfig.maxStack;
            var itemBackpackUI = GetUIItem();
            itemBackpackUI.SetItemCount(healthItem.Value);
            itemBackpackUI.SetItemName(itemConfig.displayName);

            itemBackpackUI.SetItemBPData(itemConfig.ItemType, healthItem.Value, itemConfig.GetEnumIndex());
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