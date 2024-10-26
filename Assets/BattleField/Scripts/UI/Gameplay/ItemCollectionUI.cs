using System;
using UnityEngine;
using UnityEngine.UI;


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
        itemCollectUI.SetItemCount(itemInGame.GetQuantity());
        itemCollectUI.SetItemName(itemInGame.GetItemName());

        itemCollectUI.SetOnClickEvent(() => 
        { 
            if (Backpack.instance.CanCollect() == false) return;
            RemoveItemUI(itemInGame);
            //BackpackUI.instance.AddItemUI(itemInGame);

            itemInGame.Collect();
            itemInGame.DestroyItem();
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

    
}
public interface RunTimeItem
{
    public Action<RunTimeItem> OnRemoveItemUI { get; set; }
    public bool isDisplayedUI { get; set; }
    string GetItemName();
    int GetQuantity();
    void Collect();
    void DestroyItem();
    string GetUniqueID();
}
