using Fusion;
using System;
using System.Xml;
using UnityEngine;
[Serializable]
public class CustomData
{
    public string key;
    public float value;
}

public interface ItemDataEnum
{
    Enum GetSubItemType();
    ItemType GetItemType();

    void SetQuantity(int newAmount);
}

public abstract class ItemNetworkBase<_EnumType, _Config> : NetworkBehaviour, ItemDataEnum, RunTimeItem where _EnumType : Enum where _Config : ItemConfig<_EnumType>
{
    [Networked] public int quantity { get; set; }
    public Action<RunTimeItem> OnRemoveItemUI { get; set; }
    public bool isDisplayedUI { get; set; }
    public CustomData[] customDatas;
    public _Config config;
    private BoundItem boundItem;

    public string DisplayName { get; set; }
    public int Quantity { get => quantity; set => quantity = value; }

    private void Awake()
    {
        boundItem = GetComponent<BoundItem>();
    }
    public override void Spawned()
    {
        UniqueID = Object.Id.ToString();
        DisplayName = config.displayName;
        base.Spawned();
        Invoke(nameof(BoundItemSetup), .5f);
    }

    private void BoundItemSetup()
    {
        if(boundItem == null)
        {
            Debug.LogError("Bound item is null in item", gameObject);
            return;
        }

        boundItem.SetToGround();
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        if (isDisplayedUI)
        {
            OnRemoveItemUI?.Invoke(this);
        }
    }


    public void Collect()
    {
        InventoryItem inventoryItem = new InventoryItem();
        inventoryItem.maxStack = config.maxStack;
        inventoryItem.ItemType = config.ItemType;
        inventoryItem.displayName = config.displayName;
        inventoryItem.Icon = config.Icon;
        inventoryItem.amount = quantity;
        inventoryItem.customDatas = customDatas;
        inventoryItem._SubItemEnum = config.SubItemType;
        StorageManager.instance.Add(config.ItemType, config.SubItemType, inventoryItem);

        DestroyItem();
    }

    public string UniqueID { get; set; }
    
    public void DestroyItem()
    {
        DestroyRPC();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void DestroyRPC()
    {
        if (HasStateAuthority)
        {
            Runner.Despawn(Object);
        }
    }

    public Enum GetSubItemType()
    {
        return config.SubItemType;
    }

    public ItemType GetItemType()
    {
        return config.ItemType;
    }

    public void SetQuantity(int newAmount)
    {
        SetQuantityRPC(newAmount);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void SetQuantityRPC(int newAmount)
    {
        if (Object.HasStateAuthority)
        {
            quantity = newAmount;
        }
    }

}