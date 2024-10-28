using Fusion;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class ItemConfigSettings<_EnumType,ItemDataConfig> : ScriptableObject where _EnumType : Enum where ItemDataConfig : ItemConfig<_EnumType>
{
    public List<ItemDataConfig> itemConfigs;
    public ItemType ItemType;
    public ItemDefaultConfigs ItemDefaultConfigs;
    public virtual ItemDataConfig GetItemDataConfig(_EnumType _EnumType)
    {
        return Get(itemConfigs, _EnumType);
    }
    public static A Get<A, ItemType>(List<A> itemConfigs, ItemType itemType) where ItemType : Enum where A : ItemConfig<ItemType>
    {
        foreach (var itemData in itemConfigs)
        {
            if (itemData.SubItemType.Equals(itemType))
                return itemData;
        }
        Debug.LogError($"Item Type {itemType} is does not definetions on List");

        return null;
    }
    public abstract void GetDefaultSetting();
}
[Serializable]
public class ItemConfig<SubType> where SubType : Enum
{
    public string displayName;
    public Sprite Icon;
    public SubType SubItemType;
    public int maxStack;
    public ItemType ItemType;

    public virtual void ShowDebug()
    {
        Debug.Log($"Item Name: {displayName}, MaxStack {maxStack} MainType: {ItemType.ToString()} SubType:{SubItemType.ToString()}");
    }

    public virtual int GetEnumIndex()
    {
        return (int)(object)SubItemType;
    }
}
