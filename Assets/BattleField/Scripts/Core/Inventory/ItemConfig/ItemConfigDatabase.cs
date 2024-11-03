using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemConfigDatabase", menuName = "Configs/ItemDefaults")]
public class ItemConfigDatabase : ScriptableObject
{
    public List<HealthItemConfig> healthConfigs;
    public List<AmmoItemConfig> ammoConfigs;
    public List<GunItemConfig> gunConfigs;
    public ItemType itemType;

    public ItemConfig<T> FindItem<T>(ItemType itemType, T subItemType) where T : Enum
    {
        switch (itemType)
        {
            case ItemType.Health:
                foreach (var item in healthConfigs)
                {
                    if (item.SubItemType.Equals(subItemType))
                    {
                        return item as ItemConfig<T>;
                    }
                }
                break;

            case ItemType.Ammo:
                foreach (var item in ammoConfigs)
                {
                    if (item.SubItemType.Equals(subItemType))
                    {
                        return item as ItemConfig<T>;
                    }
                }
                break;
            case ItemType.Gun:
                foreach (var item in gunConfigs)
                {
                    if (item.SubItemType.Equals(subItemType))
                    {
                        return item as ItemConfig<T>;
                    }
                }
                break;
        }

        Debug.LogWarning($"No item found with ItemType: {itemType} and SubItemType: {subItemType}");
        return null;
    }

}

