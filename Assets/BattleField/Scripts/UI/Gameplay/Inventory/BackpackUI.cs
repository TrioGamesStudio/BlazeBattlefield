using System;
using Unity.VisualScripting;
using UnityEngine;
public class BackpackUI : BaseTest<ItemData>
{
    public static BackpackUI instance;
    public BackpackButtonGroupUI backpackButtonGroupUI;
    private int currentItemIndex;
    protected override void Init()
    {
        base.Init();
        instance = this;
        backpackButtonGroupUI.gameObject.SetActive(false);
    }
    protected override void ConfigureItemUI(ItemData customObject, ItemCollectUI itemCollectUI)
    {
        itemCollectUI.SetItemName(customObject.itemName);
        itemCollectUI.SetItemCount(customObject.count);
        itemCollectUI.SetOnClickEvent(() =>
        {
            var newIndex = itemCollectUI.transform.GetSiblingIndex();
            bool isItemActive = newIndex == currentItemIndex;
            if(isItemActive)
            {
                ResetBackpackButtonGroup();
                return;
            }
            currentItemIndex = newIndex;
            backpackButtonGroupUI.ShowByType(customObject, currentItemIndex);

            backpackButtonGroupUI.RemoveAllRegister();

            backpackButtonGroupUI.dropButton.onClick.AddListener(Drop);
            backpackButtonGroupUI.dropAllButton.onClick.AddListener(DropAll);
            backpackButtonGroupUI.useButton.onClick.AddListener(Use);
            backpackButtonGroupUI.equipButton.onClick.AddListener(Equip);

        });
    }
    private void ResetBackpackButtonGroup()
    {
        Debug.Log("Hide backpack button group",gameObject);
        backpackButtonGroupUI.Hide();
        currentItemIndex = -1;
    }

    private void Drop()
    {
        Debug.Log("Drop");
    }
    private void DropAll()
    {
        Debug.Log("Drop All");
    }
    private void Use()
    {
        Debug.Log("Use");
    }
    private void Equip()
    {
        Debug.Log("Equip");
    }
    public override void RemoveItem(ItemData customObject)
    {
        RemoveItemFromDictionary(customObject.indentifyID, customObject);
        ResetBackpackButtonGroup();
    }

    public override void AddItem(ItemData customObject)
    {
        AddItemToDictionary(customObject.indentifyID, customObject);
        ResetBackpackButtonGroup();
    }
}