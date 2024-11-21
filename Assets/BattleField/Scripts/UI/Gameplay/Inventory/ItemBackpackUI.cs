using System;
using UnityEngine;

public class ItemBackpackUI : BaseUIItem, IPoolCallback<ItemBackpackUI>
{
    public Action<ItemBackpackUI> OnCallback { get; set; }
    private InventoryItem currentItem;
    public GameObject highlightObject;
    protected override void Awake()
    {
        base.Awake();
        OnClickButton.onClick.AddListener(ShowButtonBelowItemUI);
        highlightObject.gameObject.SetActive(false);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        OnClickButton.onClick.RemoveListener(ShowButtonBelowItemUI);
    }
    private void ShowButtonBelowItemUI()
    {
        BackpackUI.instance.SetCurrentItem(currentItem);
        BackpackUI.instance.ShowButton(this);
    }

    public void OnRelease()
    {
        currentItem.OnUpdateDataAction = null;
        currentItem = null;
        OnCallback?.Invoke(this);
    }

    public void Initialize(InventoryItem item)
    {
        currentItem = item;
        currentItem.OnUpdateDataAction = UpdateData;
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

    public void Highlight()
    {
        highlightObject.gameObject.SetActive(true);
    }

    public void UnHighlight()
    {
        highlightObject.gameObject.SetActive(false);

    }
}
