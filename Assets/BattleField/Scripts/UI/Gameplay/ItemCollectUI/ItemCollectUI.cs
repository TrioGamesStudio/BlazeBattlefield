using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemCollectUI : BaseUIItem, IPoolCallback<ItemCollectUI>
{
    public Action<ItemCollectUI> OnCallback { get; set; }

    public void OnRelease()
    {
        OnCallback?.Invoke(this);
    }

    public void Initialize(RunTimeItem itemInGame)
    {
        SetItemCount(itemInGame.Quantity);
        SetItemName($"Name: {itemInGame.DisplayName}");
    }
}

public class BaseUIItem : MonoBehaviour
{
    [SerializeField] protected Image background;
    [SerializeField] protected Image icon;
    [SerializeField] protected Image durability;
    [SerializeField] protected TextMeshProUGUI itemName;
    [SerializeField] protected TextMeshProUGUI itemCount;
    [SerializeField] protected Button OnClickButton;
    private event Action OnClickButtonCallback;

    protected virtual void Awake()
    {
        OnClickButton.onClick.AddListener(RaiseCallback);
    }

    protected virtual void OnDestroy()
    {
        OnClickButton.onClick.RemoveListener(RaiseCallback);
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

    public virtual void RaiseCallback()
    {
        OnClickButtonCallback?.Invoke();
    }

}
