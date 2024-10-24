using System;
using Unity.VisualScripting;
using UnityEngine;
public class BackpackUI : BaseTest<ItemLocalData>
{
    public static BackpackUI instance;
    public BackpackButtonGroupUI backpackButtonGroupUI;
    public DropAmountUI dropAmountUI;
    private int currentItemIndex;
    protected override void Init()
    {
        base.Init();
        instance = this;
        backpackButtonGroupUI.Hide();
        dropAmountUI.Hide();
        SetupButtonListeners();
    }
    private void SetupButtonListeners()
    {
        backpackButtonGroupUI.dropButton.onClick.AddListener(HandleDropButton);
        backpackButtonGroupUI.dropAllButton.onClick.AddListener(HandleDropAllButton);
        backpackButtonGroupUI.useButton.onClick.AddListener(Use);
        backpackButtonGroupUI.equipButton.onClick.AddListener(Equip);

        dropAmountUI.OnDropItemCallback = OnDropConfirmed;

    }

    protected override void ConfigureItemUI(ItemLocalData customObject, ItemCollectUI itemCollectUI)
    {
        itemCollectUI.SetItemName(customObject.ItemName);
        itemCollectUI.SetItemCount(customObject.CurrentQuantity);
        itemCollectUI.SetOnClickEvent(() =>
        {
            var newIndex = itemCollectUI.transform.GetSiblingIndex();
            SelectItem(newIndex,customObject);
        });
    }
    private void SelectItem(int index, ItemLocalData customObject)
    {
        if (index == currentItemIndex)
        {
            ResetSelection();
            return;
        }

        currentItemIndex = index;
        UpdateUIForSelectedItem(customObject);
    }
    private void UpdateUIForSelectedItem(ItemLocalData customObject)
    {
        backpackButtonGroupUI.ShowByIndex(currentItemIndex);
        backpackButtonGroupUI.SetCurrentItem(customObject);
    }


    private void HandleDropButton()
    {
        var customObject = backpackButtonGroupUI.GetCurrentItem();
        dropAmountUI.SetupView(customObject);
        dropAmountUI.Show();
    }
    private void OnDropConfirmed(int dropAmount)
    {
        Debug.Log("Drop item: " + dropAmount);
        var currentItem = backpackButtonGroupUI.GetCurrentItem();
        Backpack.instance.DropItemAmount(currentItem,dropAmount);
        dropAmountUI.Hide();
    }
 
    private void HandleDropAllButton()
    {
        Debug.Log("Drop All", gameObject);
        var customObject = backpackButtonGroupUI.GetCurrentItem();
        Backpack.instance.Drop(customObject);
        RemoveItemUI(customObject);
    }
    private void DropAll()
    {

    }
    private void ResetSelection()
    {
        Debug.Log("Hide backpack button group", gameObject);
        backpackButtonGroupUI.Hide();
        dropAmountUI.Hide();
        currentItemIndex = -1;
    }

    private void Use()
    {
        Debug.Log("Use");
    }
    private void Equip()
    {
        Debug.Log("Equip");
    }
    public override void RemoveItemUI(ItemLocalData customObject)
    {
        RemoveItemFromDictionary(customObject.ItemIdentifier, customObject);
        ResetSelection();
    }

    public override void AddItemUI(ItemLocalData customObject)
    {
        AddItemToDictionary(customObject.ItemIdentifier, customObject);
        ResetSelection();
    }
}