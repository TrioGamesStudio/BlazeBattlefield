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
    protected override void ConfigureItemUI(ItemData customObject, ItemCollectUI itemCollectUI)
    {
        itemCollectUI.SetItemName(customObject.itemName);
        itemCollectUI.SetItemCount(customObject.count);
        itemCollectUI.SetOnClickEvent(OnClickButton);
    }


    private void OnClickButton()
    {
        Debug.Log("Show two button");
    }

    public override void RemoveItem(ItemData customObject)
    {
        AddItemToDictionary(customObject.indentifyID, customObject);
    }

    public override void AddItem(ItemData customObject)
    {
        RemoveItemFromDictionary(customObject.indentifyID, customObject);
    }
}