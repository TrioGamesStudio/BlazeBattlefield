using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ItemCollectionUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private ItemCollectUI ItemCollectUIPrefab;
    [SerializeField] private GameObject view;
    [SerializeField] private GameObject content;
    [Header("Button")]
    [SerializeField] private Button toggleViewButton;
    [Header("Sprite")]
    [SerializeField] private Sprite closeSprite;
    [SerializeField] private Sprite openSprite;
    [Header("Settings")]
    [SerializeField] private bool canCollect = false;
    
    private List<ItemCollectUI> poolItemUI = new();
    public static Action<List<ItemCollectUI.ItemData>> OnItemRefreshChange;

    private void Awake()
    {
        ItemCollectUIPrefab.gameObject.SetActive(false);
        
        OnItemRefreshChange = ItemRefreshChange;
        toggleViewButton.onClick.AddListener(ToggleView);

    }

    private void OnDestroy()
    {
        OnItemRefreshChange = null;
        toggleViewButton.onClick.RemoveListener(ToggleView);
    }



    private void ToggleView()
    {
        bool isOpen = view.gameObject.activeSelf;
        view.gameObject.SetActive(!isOpen);
        // set sprite
        Sprite buttonSprite = view.gameObject.activeSelf ? openSprite : closeSprite;
        toggleViewButton.image.sprite = buttonSprite;

        
    }
 
    private void ItemRefreshChange(List<ItemCollectUI.ItemData> itemDatas)
    {
        foreach (var item in poolItemUI)
        {
            Destroy(item.gameObject);
        }
        poolItemUI.Clear();
        
        bool isEmpty = itemDatas == null || itemDatas.Count == 0;
        view.gameObject.SetActive(!isEmpty);
        if (isEmpty) return;
        
        foreach (var itemData in itemDatas)
        {
            // set item data to data
            ItemCollectUI itemCollectUI = Instantiate(ItemCollectUIPrefab, content.transform);
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
