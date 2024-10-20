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
        return itemData.itemName;
    }

    public int GetItemCount()
    {
        return itemData.count;
    }
}
[Serializable]
public struct ItemData
{
    public int count;
    public string itemName;
    public ItemDataSO ItemDataSO;

    // Constructor
    public ItemData(ItemDataSO ItemDataSO,int count)
    {
        this.ItemDataSO = ItemDataSO;
        itemName = ItemDataSO.ItemName;
        this.count = count;
    }

    
}
