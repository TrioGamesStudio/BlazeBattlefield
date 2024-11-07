using Fusion;
using System;
using System.Xml;
using Unity.VisualScripting;
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

public abstract class ItemNetworkBase<_EnumType, _Config> : NetworkBehaviour, ItemDataEnum, 
    RunTimeItem where _EnumType : Enum where _Config : ItemConfig<_EnumType>
{
    [Networked] public int quantity { get; set; }
    public Action<RunTimeItem> OnRemoveItemUI { get; set; }
    public bool isDisplayedUI { get; set; }
    public CustomData[] customDatas;
    public _Config config;
    private BoundItem boundItem;


    private void Awake()
    {
        boundItem = GetComponent<BoundItem>();
    }
    public override void Spawned()
    {
        base.Spawned();
        Invoke(nameof(BoundItemSetup), .5f);
    }

    public void BoundItemSetup()
    {
        if(boundItem == null)
        {
            Debug.LogError("Bound item is null in item", gameObject);
            return;
        }

        boundItem.Setup();
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

        //inventoryItem.maxStack = config.maxStack;
        //inventoryItem.ItemType = config.ItemType;
        //inventoryItem.displayName = config.displayName;
        //inventoryItem.Icon = config.Icon;
        //inventoryItem.amount = quantity;
        //inventoryItem._SubItemEnum = config.SubItemType;
        AddToStorage();

        DestroyItem();
    }

    protected virtual void AddToStorage()
    {
        InventoryItem inventoryItem = new();
        inventoryItem.Create(config, quantity);
        StorageManager.instance.Add(config.ItemType, config.SubItemType, inventoryItem);
    }

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
    [EditorButton]
    public void DestroyTest()
    {
        Object.RequestStateAuthority();
        DestroyRPC();
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

    public string DisplayName()
    {
        return config.displayName;
    }

    string RunTimeItem.UniqueID()
    {
        return Object.Id.ToString();
    }

    public int Quantity()
    {
        return quantity;
    }

    public Sprite GetIcon()
    {
        return config.Icon;
    }
}
