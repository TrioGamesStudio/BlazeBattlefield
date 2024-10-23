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
    public Action OnItemRemovedFromUICallback;
    [SerializeField] private ItemLocalData localItemData;

    [Networked, SerializeField, OnChangedRender(nameof(OnNetworkedItemDataChanged))]
    private ItemDataNetwork NetworkedItemData { get; set; }

    public bool IsDisplayedInUI = false;
    public string ItemName => localItemData.ItemName;
    public int ItemCount => localItemData.CurrentQuantity;
    public string ItemIdentifier => localItemData.ItemIdentifier;
    public override void Spawned()
    {
        base.Spawned();
        InitializeLocalItemData();
    }

    private void InitializeLocalItemData()
    {
        var dataSO = ItemGeneratorManager.instance.GetItemDataSO(NetworkedItemData.ItemDataSOName.ToString());
        if (dataSO == null) return;
        localItemData = new ItemLocalData(dataSO,
            NetworkedItemData.CurrentCount);
    }

    public void SetItemNetworkData(ItemDataNetwork _ItemDataNetwork)
    {
        NetworkedItemData = _ItemDataNetwork;
    }

    private void OnNetworkedItemDataChanged()
    {
        UpdateLocalItemCount();
        UpdateUIIfNeeded();
    }
    private void UpdateLocalItemCount()
    {
        localItemData.SetQuantity(NetworkedItemData.CurrentCount);
    }

    private void UpdateUIIfNeeded()
    {
        if (!IsDisplayedInUI) return;

        ItemCollectionUI.instance.UpdateUI(
            localItemData.ItemIdentifier,
            this);
    }
    public void OnItemCollected()
    {
        AddToBackpack();
        RemoveFromUI();
        DespawnItem();
    }

    private void AddToBackpack()
    {
        Backpack.instance.AddItemToInventory(localItemData);
    }

    private void RemoveFromUI()
    {
        OnItemRemovedFromUICallback?.Invoke();
    }

    private void DespawnItem()
    {
        Runner.Despawn(Object);
    }
}