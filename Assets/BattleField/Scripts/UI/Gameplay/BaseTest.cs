using System.Collections.Generic;
using UnityEngine;
public interface I_Data
{
    string GetID();
}
public abstract class BaseTest<CustomObject> : MonoBehaviour
{
    protected UnityPool<ItemCollectUI> poolItemsUI;
    protected Dictionary<string, ItemCollectUI> activeItemUIs;
    //[SerializeField] protected List<ItemCollectUI> usingList;
    [SerializeField] protected ItemCollectUI itemCollectUIPrefab;
    [SerializeField] protected GameObject content;

    protected virtual void Awake()
    {
        Init();
    }
    protected virtual void OnDestroy()
    {
        ClearAllItems();
    }
    protected virtual void Init()
    {
        Debug.Log("Init UI Game Item", gameObject);
        poolItemsUI = new UnityPool<ItemCollectUI>(itemCollectUIPrefab, 15, content.transform);
        activeItemUIs = new();
        itemCollectUIPrefab.gameObject.SetActive(false);

        if (content == null)
        {
            Debug.LogError("This content transform is null", gameObject);
        }

        if (itemCollectUIPrefab == null)
        {
            Debug.LogError("Item UI Prefab is null", gameObject);
        }

    }
    public void ClearAllItems()
    {
        // clear all
        foreach(var item in activeItemUIs.Values)
        {
            item.OnRelease();
        }
        activeItemUIs.Clear();
        poolItemsUI.Clear();
    }

    public abstract void RemoveItem(CustomObject customObject);

    public abstract void AddItem(CustomObject customObject);
    protected abstract void ConfigureItemUI(CustomObject customObject, ItemCollectUI itemCollectUI);

    protected virtual void AddItemToDictionary(string key, CustomObject customObject)
    {
        if (customObject == null)
        {
            Debug.Log("Item Null");
            return;
        }
        if (activeItemUIs.ContainsKey(key))
        {
            Debug.Log("Is Contain Key");
            return;
        }
        var itemUI = poolItemsUI.Get();
        activeItemUIs.Add(key, itemUI);
        ConfigureItemUI(customObject, itemUI);
        OnItemAdded(customObject);
    }
    protected virtual void RemoveItemFromDictionary(string key, CustomObject customObject)
    {
        if (customObject == null)
        {
            Debug.Log("Item Null");
            return;
        }
        if (!activeItemUIs.TryGetValue(key, out var ui))
        {
            Debug.Log("Not Contain Key");
            return;
        }
        activeItemUIs.Remove(key);
        ui.OnRelease();
        OnItemRemoved(customObject);
    }
    
    protected virtual void OnItemAdded(CustomObject customObject)
    {
    }

    protected virtual void OnItemRemoved(CustomObject customObject)
    {
    }
}