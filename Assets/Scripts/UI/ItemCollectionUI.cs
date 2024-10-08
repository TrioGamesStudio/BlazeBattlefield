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
            // set item data to data
            ItemCollectUI itemCollectUI = Instantiate(ItemCollectUIPrefab, Holder.transform);
            itemCollectUI.itemCount.text = itemData.count.ToString();
            itemCollectUI.itemName.text = itemData.name;
            itemCollectUI.icon.sprite = itemData.sprite;
            itemCollectUI.OnCollectCallback = () =>
            {
                Debug.Log("On Click ui collect");
                itemData.collectCallback?.Invoke();
                itemCollectUI.gameObject.SetActive(false);
            };
            itemCollectUI.gameObject.SetActive(true);

            poolItemUI.Add(itemCollectUI);
        }

    }

  
}
