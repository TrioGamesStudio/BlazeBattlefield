using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDefaultConfigs", menuName = "Configs/ItemDefaults")]
public class ItemDefaultConfigs : ScriptableObject
{
    public List<HealthItemConfig> healthConfigs;
    public List<AmmoItemConfig> ammoConfigs;

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
        }

        Debug.LogWarning($"No item found with ItemType: {itemType} and SubItemType: {subItemType}");
        return null;
    }



    [Button]
    private void LoadDefault()
    {
        switch (itemType)
        {
            case ItemType.Gun:
                break;
            case ItemType.Melee:
                break;
            case ItemType.Ammo:
                ammoConfigs.Clear();
                ammoConfigs = ItemConfigDefaultSettings.LoadAmmo();
                break;
            case ItemType.Health:
                healthConfigs.Clear();
                healthConfigs = ItemConfigDefaultSettings.LoadHealth();
                break;
            case ItemType.Grenade:
                break;
            default:
                break;
        }
        
    }
}

