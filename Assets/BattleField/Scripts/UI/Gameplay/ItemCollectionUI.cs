using UnityEngine;
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


    protected override void ConfigureItemUI(ItemInGame itemInGame, ItemCollectUI itemCollectUI)
    {
        itemCollectUI.SetItemCount(itemInGame.ItemCount);
        itemCollectUI.SetItemName(itemInGame.ItemName);

        itemCollectUI.SetOnClickEvent(() => { ButtonClick(itemInGame); });

        itemCollectUI.gameObject.SetActive(true);
    }

    private void ButtonClick(ItemInGame itemInGame)
    {
        Debug.Log("On ITem in UI clicked");
        if (Backpack.instance.CanCollect() == false) return;
        itemInGame.OnItemCollected();
    }

    protected override void OnItemAdded(ItemInGame customObject)
    {
        base.OnItemAdded(customObject);
        customObject.OnItemRemovedFromUICallback = () => RemoveItemUI(customObject);
    }

    public override void RemoveItemUI(ItemInGame customObject)
    {
        RemoveItemFromDictionary(customObject.ItemIdentifier, customObject);
    }

    public override void AddItemUI(ItemInGame customObject)
    {
        AddItemToDictionary(customObject.ItemIdentifier, customObject);
    }

    
}
