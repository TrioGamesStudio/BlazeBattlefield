using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public class ItemCollectionUI : MonoBehaviour
{
    public ItemCollectUI ItemCollectUIPrefab;
    public GameObject View;
    [FormerlySerializedAs("Holder")] public GameObject Content;
    private List<ItemCollectUI> poolItemUI = new();
    public static Action<List<ItemCollectUI.ItemData>> OnItemRefreshChange;
    public bool canCollect = false;

    private void Awake()
    {
        ItemCollectUIPrefab.gameObject.SetActive(false);
        
        OnItemRefreshChange = ItemRefreshChange;
    }

    private void OnDestroy()
    {
        OnItemRefreshChange = null;
    }
    // fake data
    public class ItemRaw
    {
        public ItemRaw(string name, int count)
        {
            this.name = name;
            this.count = count;
        }

        public string itemId;
        public string name;
        public int count;
    }
    [Button]
    private void DemoTest()
    {
        
        List<ItemRaw> itemList = new List<ItemRaw>()
        {
            new ItemRaw("Gun 1", 2),
            new ItemRaw("Gun 2", 2),
            new ItemRaw("Gun 3", 2),
            new ItemRaw("Gun 4", 2),
            new ItemRaw("Gun 5", 2),
        };
        List<ItemCollectUI.ItemData> itemDatas = new List<ItemCollectUI.ItemData>();
        ItemCollectionUI.OnItemRefreshChange?.Invoke(itemDatas);
        foreach (var item in itemList)
        {
            itemDatas.Add(
                ItemCollectUI.ItemData.CreateInstance(null,
                    item.name,
                    item.count, () =>
            {
                // Inventory.Collect(item.itemID)
                // hay dong bo lai list item cua nguoi choi khac
            }));
        }

        ItemRefreshChange(itemDatas);
    }

    private void ItemRefreshChange(List<ItemCollectUI.ItemData> itemDatas)
    {
        foreach (var item in poolItemUI)
        {
            Destroy(item.gameObject);
        }
        poolItemUI.Clear();
        
        bool isEmpty = itemDatas == null || itemDatas.Count == 0;
        View.gameObject.SetActive(!isEmpty);
        if (isEmpty) return;
        
        foreach (var itemData in itemDatas)
        {
            // set item data to data
            ItemCollectUI itemCollectUI = Instantiate(ItemCollectUIPrefab, Content.transform);
            itemCollectUI.itemCount.text = itemData.count.ToString();
            itemCollectUI.itemName.text = itemData.name;
            itemCollectUI.icon.sprite = itemData.sprite;
            itemCollectUI.OnCollectCallback = () =>
            {
                if (canCollect)
                {
                    itemData.collectCallback?.Invoke();
                    itemCollectUI.gameObject.SetActive(false);
                }
            };
            itemCollectUI.gameObject.SetActive(true);

            poolItemUI.Add(itemCollectUI);
        }

    }

  
}
