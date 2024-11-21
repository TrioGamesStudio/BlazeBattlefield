using NaughtyAttributes;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class ItemCollectionUI : BaseTest<IRunTimeItem>
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


    protected override void ConfigureItemUI(IRunTimeItem itemInGame, ItemCollectUI itemCollectUI)
    {
        itemCollectUI.Initialize(itemInGame);
        //itemCollectUI.SetItemCount(itemInGame.GetQuantity());
        itemCollectUI.SetOnClickEvent(() => 
        {
            // need to check storage can collect item
            RemoveItemUI(itemInGame);
            itemInGame.Collect();
        });
        itemInGame.OnRemoveItemUI = RemoveItemUI;
        itemCollectUI.gameObject.SetActive(true);
    }
 
    public override void RemoveItemUI(IRunTimeItem customObject)
    {
        RemoveItemFromDictionary(customObject.UniqueID(), customObject);
    }

    public override void AddItemUI(IRunTimeItem customObject)
    {
        AddItemToDictionary(customObject.UniqueID(), customObject);
    }

    [Button]
    public void GetFirstItemDebug()
    {
        if (activeItemUIs.Count == 0)
        {
            Debug.Log("Bay gio khong co item nao trong danh sach thu thap", gameObject);
            return;
        }
        if (activeItemUIs.Count == 0) return;
        var itemUI = activeItemUIs.First().Value;
        itemUI.RaiseCallback();
        //Debug.Log("Show first Item", itemUI.gameObject);
    }

}
