using System;
using Fusion;
using UnityEngine;
[Serializable]
public struct ItemDataNetwork : INetworkStruct
{
    public NetworkString<_8> ItemDataSOName;
    [Networked] public int CurrentCount { get; set; }
}

public class ItemInGame : NetworkBehaviour
{
    public Action OnRemoveUICallback;
    [SerializeField] private ItemData itemData;
    [Networked,SerializeField] private ItemDataNetwork ItemDataNetwork { get; set; }
    public void Setup(ItemDataNetwork _ItemDataNetwork)
    {
        ItemDataNetwork = _ItemDataNetwork;
        itemData = new ItemData(ItemGeneratorManager.instance.GetItemDataSO(ItemDataNetwork.ItemDataSOName.ToString()),
            ItemDataNetwork.CurrentCount);
        // itemData = new ItemData(itemDataSo, count);
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
        return itemData.GetItemName();
    }

    public int GetItemCount()
    {
        return itemData.GetCount();
    }

    public string GetKey()
    {
        return itemData.GetIndentifyID();
    }
}