using Fusion;
using NaughtyAttributes;
using System;
using static Fusion.Allocator;
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

public abstract class ItemNetworkBase<_EnumType, T> : NetworkBehaviour, ItemDataEnum, RunTimeItem where _EnumType : Enum where T : ItemConfig<_EnumType>
{
    [Networked] public int quantity { get; set; }
    public Action<RunTimeItem> OnRemoveItemUI { get; set; }
    public bool isDisplayedUI { get; set; }

    public ItemConfigSettings<_EnumType, T> ItemConfigSettings;
    public _EnumType _enumType;
    public CustomData[] customDatas;
    public ItemConfig<_EnumType> config;
    public override void Spawned()
    {
        base.Spawned();
        SetConfig();
    }

    public void SetConfig()
    {
        config = ItemConfigSettings.GetItemDataConfig(_enumType);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        if (isDisplayedUI)
        {
            OnRemoveItemUI?.Invoke(this);
        }
    }

    public string GetItemName()
    {
        return config.displayName;
    }

    public int GetQuantity()
    {
        return quantity;
    }

    public void Collect()
    {
        InventoryItem inventoryItem = new InventoryItem();
        inventoryItem.maxStack = config.maxStack;
        inventoryItem.displayName = config.displayName;
        inventoryItem.Icon = config.Icon;
        inventoryItem.amount = quantity;
        inventoryItem.customDatas = customDatas;
        inventoryItem._SubItemEnum = _enumType;
        StorageManager.instance.Add(ItemConfigSettings.ItemType, _enumType, inventoryItem);

        DestroyItem();
    }


    public string GetUniqueID()
    {
        return Object.NetworkTypeId.ToString();
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

    public Enum GetSubItemType()
    {
        return _enumType;
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