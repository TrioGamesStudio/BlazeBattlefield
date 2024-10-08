using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class ItemCollectionUI : MonoBehaviour
{
    public ItemCollectUI ItemCollectUIPrefab;
    public GameObject Holder;
    private List<ItemCollectUI> poolItemUI = new();
    public static Action<List<ItemCollectUI.ItemData>> OnItemRefreshChange;
    private void Awake()
    {
        ItemCollectUIPrefab.gameObject.SetActive(false);
        
        OnItemRefreshChange = ItemRefreshChange;
    }

    private void OnDestroy()
    {
        OnItemRefreshChange = null;
    }
    [Button]
    private void TestUI()
    {
        List<ItemCollectUI.ItemData> list = new List<ItemCollectUI.ItemData>()
        {
            ItemCollectUI.ItemData.CreateInstance(null, "Gun 1", 3),
            ItemCollectUI.ItemData.CreateInstance(null, "Gun 3", 1),
            ItemCollectUI.ItemData.CreateInstance(null, "Dagger 3", 1),
            ItemCollectUI.ItemData.CreateInstance(null, "Sniper 0", 2),
            ItemCollectUI.ItemData.CreateInstance(null, "Blade 1", 6),
        };
        ItemRefreshChange(list);
    }
    private void ItemRefreshChange(List<ItemCollectUI.ItemData> itemDatas)
    {
        foreach (var item in poolItemUI)
        {
            Destroy(item.gameObject);
        }
        poolItemUI.Clear();
        
        if (itemDatas == null || itemDatas.Count == 0) return;
        
        foreach (var itemData in itemDatas)
        {
            ItemCollectUI itemCollectUI = Instantiate(ItemCollectUIPrefab, Holder.transform);
            itemCollectUI.itemCount.text = itemData.count.ToString();
            itemCollectUI.itemName.text = itemData.name;
            itemCollectUI.icon.sprite = itemData.sprite;
            
            itemCollectUI.gameObject.SetActive(true);

            poolItemUI.Add(itemCollectUI);
        }

    }

  
}
