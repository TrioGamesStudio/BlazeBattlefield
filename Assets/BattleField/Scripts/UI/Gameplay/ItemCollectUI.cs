using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemCollectUI : MonoBehaviour, IPointerClickHandler
{
    [Serializable]
    public class ItemData
    {
        public static ItemData CreateInstance(Sprite sprite,string name,int count, Action callback)
        {
            var itemData = new ItemData();
            itemData.sprite = sprite;
            itemData.name = name;
            itemData.count = count;
            itemData.collectCallback = callback;
            return itemData;
        }

        public Sprite sprite;
        public string name;
        public int count;
        public Action collectCallback;
    }
    public Image background;
    public Image icon;
    public Image durability;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemCount;
    public Action OnCollectCallback;
    public void OnPointerClick(PointerEventData eventData)
    {
        // Debug.Log($"UI Collect: '{itemName.text}' '{itemCount.text}'",gameObject);
        OnCollectCallback?.Invoke();
        // Inventory.Collect(item.id)
    }
}

