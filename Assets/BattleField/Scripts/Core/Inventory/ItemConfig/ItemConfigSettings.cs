using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemConfigSettings<_EnumType,ItemDataConfig> : ScriptableObject where _EnumType : Enum where ItemDataConfig : ItemConfig<_EnumType>
{
    public List<ItemDataConfig> itemConfigs;
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
}
[Serializable]
public class ItemConfig<T> where T : Enum
{
    public string displayName;
    public Sprite Icon;
    public T SubItemType;
    public int maxStack;
    public ItemType ItemType;
}
