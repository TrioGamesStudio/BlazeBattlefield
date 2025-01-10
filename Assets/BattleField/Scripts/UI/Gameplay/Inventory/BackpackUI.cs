using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BackpackUI : MonoBehaviour
{
    public static BackpackUI instance;
    protected UnityPool<ItemBackpackUI> poolItemsUI;
    [SerializeField] protected ItemBackpackUI itemCollectUIPrefab;
    [SerializeField] protected GameObject content;
    [SerializeField] protected BackpackButtonGroupUI buttonGroupUI;
    [SerializeField] protected DropAmountUI dropAmountUI;

    [SerializeField] private Button acceptDropAllBtn;
    [SerializeField] private Button deliceDropAllBtn;
    [SerializeField] private GameObject acceptDropPanel;

    [SerializeField] protected int dropCount;
    private InventoryItem currentItem;

    private Dictionary<ItemType, List<ItemBackpackUI>> _itemUIs = new();

    public void Setup()
    {
        instance = this;
        itemCollectUIPrefab.gameObject.SetActive(false);
        poolItemsUI = new UnityPool<ItemBackpackUI>(itemCollectUIPrefab, 10, content.transform);
        StorageManager.OnAddItem += HandleItemAdded;
        StorageManager.OnRemoveItem += HandleItemRemove;


        HideButton();
        HideAcceptDropPanel();
        buttonGroupUI.SetOnDropFull(ShowAcceptDropPanel);
        buttonGroupUI.SetOndropItemAmount(ShowDropAmount);
        buttonGroupUI.SetOnUseItem(ActiveHealthTimer);
        dropAmountUI.Hide();
        dropAmountUI.SetAcceptDrop(OnAcceptDrop);

        acceptDropAllBtn.onClick.AddListener(DropAllItem);
        deliceDropAllBtn.onClick.AddListener(HideAcceptDropPanel);
    }


    private void OnDestroy()
    {
        StorageManager.OnAddItem -= HandleItemAdded;
        StorageManager.OnRemoveItem -= HandleItemRemove;

        buttonGroupUI.SetOnDropFull(null);
        buttonGroupUI.SetOndropItemAmount(null);

        dropAmountUI.SetAcceptDrop(null);


        acceptDropAllBtn.onClick.RemoveListener(DropAllItem);
        deliceDropAllBtn.onClick.RemoveListener(HideAcceptDropPanel);
    }

    private void ShowAcceptDropPanel()
    {
        acceptDropPanel.gameObject.SetActive(true);
    }
    private void HideAcceptDropPanel()
    {
        acceptDropPanel.gameObject.SetActive(false);
    }

    public void ActiveHealthTimer()
    {
        if (currentItem == null) return;
        if(NetworkPlayer.Local.GetComponent<HPHandler>().Networked_HP < 100)
        {
            var healthConfig = ItemDatabase.instance.ItemConfigDatabase.FindHealthItem(currentItem._SubItemEnum);
            healthAmount = healthConfig.healthAmount;
            TimerActionHandler.instance.StartTimer(healthConfig.usingTime, () => { OnUseHealthItem(currentItem); }, null);
        }
    }
    private byte healthAmount = 0;
    private void OnUseHealthItem(InventoryItem currentItem)
    {
        
        currentItem.amount -= 1;
        currentItem?.OnUpdateData();
        NetworkPlayer.Local.GetComponent<HPHandler>().OnHealRPC(healthAmount);
        if (currentItem.amount == 0)
        {
            StorageManager.instance.Remove(currentItem.ItemType, currentItem._SubItemEnum, currentItem);
            HideButton();
        }
    }
    public void ShowDropAmount()
    {
        dropAmountUI.Show();
        dropAmountUI.SetupView(currentItem);
    }

    private void OnAcceptDrop(int newDropCount)
    {
        if (newDropCount == 0) return;

        dropCount = newDropCount;
        if(dropCount == currentItem.amount)
        {
            DropAllItem();
        }
        else
        {
            StorageManager.instance.SplitItem(currentItem, newDropCount);
        }
        HideAll();
        previousItemBackpackUI?.UnHighlight();
        Debug.Log("Drop With Count:" + newDropCount);
    }

    private void DropAllItem()
    {
        HideAll();
        StorageManager.instance.DropAll(currentItem);
    }
    private void HideAll()
    {
        HideButton();
        HideDropAmount();
        HideAcceptDropPanel();
    }

    private void HandleItemAdded(InventoryItem item)
    {
        if(item == null)
        {
            Debug.Log("This item is null", gameObject);
            return;
        }

        var ui = poolItemsUI.Get();
        ui.Initialize(item);

        if (!_itemUIs.ContainsKey(item.ItemType))
            _itemUIs[item.ItemType] = new List<ItemBackpackUI>();
       
        _itemUIs[item.ItemType].Add(ui);
        ui.gameObject.SetActive(true);
    }

    private void HandleItemRemove(InventoryItem item)
    {
        if (_itemUIs.TryGetValue(item.ItemType, out var uis) && uis.Count > 0)
        {
            foreach (var itemUI in uis)
            {
                if (itemUI.AreSameItem(item))
                {
                    var key = item.ItemType;
                    uis.Remove(itemUI);
                    itemUI.OnRelease();
                    break;
                }
            }
        }

    }



    public void HideDropAmount()
    {
        dropAmountUI.gameObject.SetActive(false);
    }
    private int currentIndex = -1;
    private ItemBackpackUI previousItemBackpackUI;
    public void ShowButton(ItemBackpackUI itemBackpackUI)
    {
        HideDropAmount();
        buttonGroupUI.transform.parent = null;
        int transformIndex = itemBackpackUI.transform.GetSiblingIndex() + 1;
      
        if (transformIndex == currentIndex)
        {
            Debug.Log("Fuck you 3");
            previousItemBackpackUI?.UnHighlight();
            previousItemBackpackUI = null;
            HideButton();
            return;
        }
        Debug.Log("Fuck you 4");

        buttonGroupUI.transform.SetParent(content.transform);

        previousItemBackpackUI?.UnHighlight();
        previousItemBackpackUI = itemBackpackUI;
        previousItemBackpackUI.Highlight();

        buttonGroupUI.SetupView(currentItem);
        buttonGroupUI.transform.SetSiblingIndex(transformIndex);
        buttonGroupUI.gameObject.SetActive(true);
        Debug.Log($"{itemBackpackUI.transform.GetSiblingIndex()} | {transformIndex}");
        currentIndex = transformIndex;
    }
  
    public void HideButton()
    {
        currentIndex = -1;
        buttonGroupUI.gameObject.SetActive(false);
    }

    public void SetCurrentItem(InventoryItem inventoryItem)
    {
        currentItem = inventoryItem;
    }

}
