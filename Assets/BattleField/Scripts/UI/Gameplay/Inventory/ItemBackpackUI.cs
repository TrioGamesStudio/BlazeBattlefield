using System;
using UnityEngine;

public class ItemBackpackUI : BaseUIItem, IPoolCallback<ItemBackpackUI>
{
    public Action<ItemBackpackUI> OnCallback { get; set; }
    private InventoryItem currentItem;
    private bool isShow = false;
    protected override void Awake()
    {
        base.Awake();
        OnClickButton.onClick.AddListener(ShowButtonBelowItemUI);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        OnClickButton.onClick.RemoveListener(ShowButtonBelowItemUI);
    }
    private void ShowButtonBelowItemUI()
    {
        if (!isShow)
        {
            BackpackUI.instance.ShowButton(transform.GetSiblingIndex() + 1);
            BackpackUI.instance.SetCurrentItem(currentItem);
            isShow = true;
        }
        else
        {
            BackpackUI.instance.HideButton();
            isShow = false;
        }
    }

    public void OnRelease()
    {
        currentItem.OnUpdateData = null;
        currentItem = null;
        OnCallback?.Invoke(this);
    }

    public void Initialize(InventoryItem item)
    {
        currentItem = item;
        //currentItem.OnUpdateData = UpdateData;
        UpdateData();
    }

    private void UpdateData()
    {
        if (currentItem == null) return;
        Debug.Log($"Add to UI{currentItem.displayName},amoumt {currentItem.amount},item type {currentItem.ItemType},max stack {currentItem.maxStack}");

        SetItemName(currentItem.displayName);
        SetItemCount(currentItem.amount);
    }

    public bool AreSameItem(InventoryItem item)
    {
        return currentItem.Equals(item);
    }

}
