using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        itemCount.text = "x"+count.ToString();
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
