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


    protected override void ConfigureItemUI(ItemInGame itemInGame, ItemCollectUI itemCollectUI)
    {
        itemCollectUI.SetItemCount(itemInGame.GetItemCount());
        itemCollectUI.SetItemName(itemInGame.GetItemName());

        itemCollectUI.SetOnClickEvent(() => { ButtonClick(itemInGame); });

        itemCollectUI.gameObject.SetActive(true);
    }

    private void ButtonClick(ItemInGame itemInGame)
    {
        Debug.Log("On ITem in UI clicked");
        if (Backpack.instance.CanCollect() == false) return;
        itemInGame.OnCollect();
    }

    protected override void OnItemAdded(ItemInGame customObject)
    {
        base.OnItemAdded(customObject);
        customObject.OnRemoveUICallback = () => RemoveItemUI(customObject);
    }

    public override void RemoveItemUI(ItemInGame customObject)
    {
        RemoveItemFromDictionary(customObject.GetKey(), customObject);
    }

    public override void AddItemUI(ItemInGame customObject)
    {
        AddItemToDictionary(customObject.GetKey(), customObject);
    }
}
