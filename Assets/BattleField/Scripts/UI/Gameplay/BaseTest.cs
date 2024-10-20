using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTest<CustomObject> : MonoBehaviour
{
    protected UnityPool<ItemCollectUI> poolItemsUI;
    protected Dictionary<CustomObject, ItemCollectUI> dictionary;
    [SerializeField] protected List<ItemCollectUI> usingList;
    [SerializeField] protected ItemCollectUI itemCollectUIPrefab;
    [SerializeField] protected GameObject content;

    private void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        Debug.Log("Init UI Game Item", gameObject);
        poolItemsUI = new UnityPool<ItemCollectUI>(itemCollectUIPrefab, 15, content.transform);
        dictionary = new();
        usingList = new();

        if (content == null)
        {
            Debug.LogError("This content transform is null", gameObject);
        }

        if (itemCollectUIPrefab == null)
        {
            Debug.LogError("Item UI Prefab is null", gameObject);
        }

        itemCollectUIPrefab.gameObject.SetActive(false);
    }
    
    public void RemoveItem(GameObject _gameObject)
    {
        var itemInGame = _gameObject.GetComponent<CustomObject>();
        RemoveItemFromDictionary(itemInGame);
    }

    public virtual void AddItem(GameObject _gameObject)
    {
        var customObject = _gameObject.GetComponent<CustomObject>();
        if (customObject == null) return;
        AddItemToDictionary(customObject);
    }
    private void AddItemToDictionary(CustomObject itemInGame)
    {
        dictionary.Add(itemInGame, CreateUI(itemInGame));
        SetupWhenAddItem(itemInGame);
    }
    protected abstract ItemCollectUI CreateUI(CustomObject customObject);

    protected virtual void RemoveItemFromDictionary(CustomObject customObject)
    {
        if (customObject == null) return;
        if (!dictionary.TryGetValue(customObject, out var ui)) return;
        ui.OnRelease();
        dictionary.Remove(customObject);
        SetWhenRemoveItem(customObject);
    }
    
    protected virtual void SetupWhenAddItem(CustomObject customObject)
    {
    }

    protected virtual void SetWhenRemoveItem(CustomObject customObject)
    {
    }
}