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

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        if (!IsDisplayedInUI) return;
        //ItemCollectionUI.instance.RemoveItemUI(this);
    }

    private void InitializeLocalItemData()
    {
        var dataSO = ItemGeneratorManager.instance.GetItemDataSO(NetworkedItemData.ItemDataSOName.ToString());
        if (dataSO == null)
        {
            Debug.LogWarning("Item Data SO is null",gameObject);
            return;
        }
        localItemData = new ItemLocalData(dataSO,
            NetworkedItemData.CurrentCount);
    }

    public void SetItemNetworkData(ItemDataNetwork _ItemDataNetwork)
    {
        NetworkedItemData = _ItemDataNetwork;
        InitializeLocalItemData();
    }

    private void OnNetworkedItemDataChanged()
    {
        UpdateLocalItemCount();
        UpdateUIIfNeeded();
    }
    private void UpdateLocalItemCount()
    {
        // for now, just have quantity
        localItemData.SetQuantity(NetworkedItemData.CurrentCount);
    }

    private void UpdateUIIfNeeded()
    {
        // if local client displaying it in UI, then update it
        if (!IsDisplayedInUI) return;

        //ItemCollectionUI.instance.UpdateUI(
        //    localItemData.ItemIdentifier,
        //    this);
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
        RPCDespawnItem();
    }
    [Rpc(RpcSources.All,RpcTargets.StateAuthority)]
    private void RPCDespawnItem()
    {
        if (Object.HasStateAuthority) 
        {
            Runner.Despawn(Object);
        }
    }
}