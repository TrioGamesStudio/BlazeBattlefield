using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemCollectUI : MonoBehaviour, IPoolCallback<ItemCollectUI>
{
    [SerializeField] private Image background;
    [SerializeField] private Image icon;
    [SerializeField] private Image durability;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemCount;
    [SerializeField] private Button OnClickButton;
    private event Action OnClickButtonCallback;
    public Action<ItemCollectUI> OnCallback { get; set; }

    private void Awake()
    {
        OnClickButton.onClick.AddListener(RaiseCallback);
    }

    private void OnDestroy()
    {
        OnClickButton.onClick.RemoveListener(RaiseCallback);
    }

    public void OnRelease()
    {
        OnCallback?.Invoke(this);
    }

    public void SetItemName(string itemNameStr)
    {
        itemName.text = itemNameStr;
    }

    public void SetItemCount(int count)
    {
        itemCount.text = count.ToString();
    }

    public void SetIcon(Sprite icon)
    {
        this.icon.sprite = icon;
    }

    public void SetOnClickEvent(Action callback)
    {
        this.OnClickButtonCallback = callback;
    }

    private void RaiseCallback()
    {
        OnClickButtonCallback?.Invoke();
    }
}
