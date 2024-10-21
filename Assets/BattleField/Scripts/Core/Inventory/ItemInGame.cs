using System;
using Fusion;

[Serializable]
public class ItemInGame : NetworkBehaviour
{
    public Action OnRemoveUICallback;
    private ItemData itemData;
    
    public void Setup(ItemDataSO itemDataSo,int count)
    {
        itemData = new ItemData(itemDataSo,count);
    }

    public void OnCollect()
    {
        // add copy off data
        Backpack.instance.AddItemToInventory(itemData);
        // remove ui show in screen
        OnRemoveUICallback?.Invoke();
        // Destroy game object
        Runner.Despawn(Object);
    }

    public string GetItemName()
    {
        return itemData.ItemDataSO.ItemName;
    }

    public int GetItemCount()
    {
        return itemData.count;
    }

    public string GetKey()
    {
        return itemData.indentifyID;
    }
}
[Serializable]
public struct ItemData
{
    public string indentifyID;
    public int count;
    public ItemDataSO ItemDataSO;
    // Constructor
    public ItemData(ItemDataSO _ItemDataSO,int _count)
    {
        indentifyID = Guid.NewGuid().ToString();
        ItemDataSO = _ItemDataSO;
        count = _count;
    }
}
