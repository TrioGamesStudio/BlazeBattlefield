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
        public static ItemData CreateInstance(Sprite sprite,string name,int count)
        {
            var itemData = new ItemData();
            itemData.sprite = sprite;
            itemData.name = name;
            itemData.count = count;
            return itemData;
        }

        public Sprite sprite;
        public string name;
        public int count;
    }
    public Image background;
    public Image icon;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemCount;
    public Action CollectItemCallback;
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Collect");
        CollectItemCallback?.Invoke();
    }
}

