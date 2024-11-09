using System;
using UnityEngine;
using static Fusion.Allocator;

public class InventoryItem
{
    public InventoryItem Create<T>(ItemConfig<T> itemConfig, int currentAmount) where T : Enum
    {
        Config = itemConfig as ItemConfig<Enum>; 
        ItemType = itemConfig.ItemType;
        _SubItemEnum = itemConfig.SubItemType;
        displayName = itemConfig.displayName;
        Icon = itemConfig.Icon;
        maxStack = itemConfig.maxStack;
        amount = currentAmount;
        return this;
    }
    public ItemConfig<Enum> Config;
    public ItemType ItemType;
    public Enum _SubItemEnum;
    public string displayName;
    public Sprite Icon;
    public int maxStack;
    public int amount;
    public Action OnUpdateData;
}
