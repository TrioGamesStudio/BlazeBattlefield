using System;
using System.Collections;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class ItemCollectionUI : BaseTest<ItemInGame>
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

    private void OnDestroy()
    {
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


    protected override ItemCollectUI CreateUI(ItemInGame itemInGame)
    {
        ItemCollectUI itemCollectUI = poolItemsUI.Get();
        itemCollectUI.SetItemCount(itemInGame.GetItemCount());
        itemCollectUI.SetItemName(itemInGame.GetItemName());
        // itemCollectUI.icon.sprite = itemInGame.sprite;

        itemCollectUI.SetOnClickEvent(() => { ButtonClick(itemInGame); });

        itemCollectUI.gameObject.SetActive(true);
        usingList.Add(itemCollectUI);
        return itemCollectUI;
    }

    private void ButtonClick(ItemInGame itemInGame)
    {
        Debug.Log("On ITem in UI clicked");
        if (Backpack.instance.CanCollect() == false) return;
        itemInGame.OnCollect();
    }

    protected override void SetupWhenAddItem(ItemInGame customObject)
    {
        base.SetupWhenAddItem(customObject);
        customObject.OnRemoveUICallback = () => RemoveItemFromDictionary(customObject);
    }
}
