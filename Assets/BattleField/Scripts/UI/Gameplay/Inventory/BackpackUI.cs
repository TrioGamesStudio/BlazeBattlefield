using System;
using UnityEngine;

public class BackpackUI : BaseTest<ItemData>
{
    public static BackpackUI instance;
    protected override void Init()
    {
        base.Init();
        instance = this;
    }
    protected override ItemCollectUI CreateUI(ItemData customObject)
    {
        var itemCollectUI = poolItemsUI.Get();
        itemCollectUI.SetItemName(customObject.itemName);
        itemCollectUI.SetItemCount(customObject.count);
        itemCollectUI.SetOnClickEvent(OnClickButton);
        return itemCollectUI;
    }


    private void OnClickButton()
    {
        Debug.Log("Show two button");
    }
}