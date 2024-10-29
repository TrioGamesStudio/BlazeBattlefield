using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;


public class ItemCollectionUI : BaseTest<RunTimeItem>
{
    public static ItemCollectionUI instance;


    [SerializeField] private GameObject view;
    [SerializeField] private Button toggleViewButton;

    protected override void Init()
    {
        base.Init();
        toggleViewButton.onClick.AddListener(ToggleView);
        instance = this;
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
        toggleViewButton.onClick.RemoveListener(ToggleView);
    }


    private void ToggleView()
    {
        bool isOpen = view.gameObject.activeSelf;
        view.gameObject.SetActive(!isOpen);
        // set sprite
    }

    private void Show() => view.gameObject.SetActive(true);

    private void Hide() => view.gameObject.SetActive(false);


    protected override void ConfigureItemUI(RunTimeItem itemInGame, ItemCollectUI itemCollectUI)
    {
        itemCollectUI.Initialize(itemInGame);
        //itemCollectUI.SetItemCount(itemInGame.GetQuantity());
        //itemCollectUI.SetItemName($"Name: {itemInGame.GetItemName()} ({activeItemUIs.Count})");

        itemCollectUI.SetOnClickEvent(() => 
        { 
            //if (Backpack.instance.CanCollect() == false) return;
            RemoveItemUI(itemInGame);
            //BackpackUI.instance.AddItemUI(itemInGame);

            itemInGame.Collect();
        });
        itemInGame.OnRemoveItemUI = RemoveItemUI;
        itemCollectUI.gameObject.SetActive(true);
    }


    protected override void OnItemAdded(RunTimeItem customObject)
    {
        base.OnItemAdded(customObject);
    }

    public override void RemoveItemUI(RunTimeItem customObject)
    {
        RemoveItemFromDictionary(customObject.GetUniqueID(), customObject);
    }

    public override void AddItemUI(RunTimeItem customObject)
    {
        AddItemToDictionary(customObject.GetUniqueID(), customObject);
    }

    [Button]
    public void GetFirstItemDebug()
    {
        if (activeItemUIs.Count == 0)
        {
            Debug.Log("Bay gio khong co item nao trong danh sach thu thap", gameObject);
            return;
        }
        var itemUI = GetFirestItem();
        itemUI.RaiseCallback();
        Debug.Log("Show first Item", itemUI.gameObject);
    }

    private ItemCollectUI GetFirestItem()
    {
        foreach (var item in activeItemUIs)
        {
            return item.Value;
        }
        return null;
    }

}
public interface RunTimeItem
{
    public bool isDisplayedUI { get; set; }
    public Action<RunTimeItem> OnRemoveItemUI { get; set; }
    string GetItemName();
    int GetQuantity();
    void Collect();
    void DestroyItem();
    string GetUniqueID();
}
