using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
public class ItemBackpackUI : ItemCollectUI
{
    public int quantity;
    public ItemConfig<Enum> itemConfig;
}
public class BackpackUI : MonoBehaviour
{
    protected UnityPool<ItemCollectUI> poolItemsUI;
    [SerializeField] protected ItemBackpackUI itemCollectUIPrefab;
    [SerializeField] protected GameObject content;
    [SerializeField] protected HealthItemData healthItemData;
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
            var itemBackpackUI = poolItemsUI.Get();
            itemBackpackUI.quan
        }
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