using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemConfigDatabase", menuName = "Configs/ItemDefaults")]
public class ItemConfigDatabase : ScriptableObject
{
    [SerializeField] private List<HealthItemConfig> healthConfigs;
    [SerializeField] private List<AmmoItemConfig> ammoConfigs;
    [SerializeField] private List<GunItemConfig> gunConfigs;
    [SerializeField] private ItemType itemType;

    

    public HealthItemConfig FindHealthItem<T>(T subItemType) where T : Enum
    {
        foreach (var item in healthConfigs)
        {
            if (item.SubItemType.Equals(subItemType))
            {
                return item;
            }
        }
        Debug.LogWarning($"No health item found with SubItemType: {subItemType}");
        return null;
    }

    public AmmoItemConfig FindAmmoItem<T>(T subItemType) where T : Enum
    {
        foreach (var item in ammoConfigs)
        {
            if (item.SubItemType.Equals(subItemType))
            {
                return item;
            }
        }
        Debug.LogWarning($"No ammo item found with SubItemType: {subItemType}");
        return null;
    }

    public GunItemConfig FindGunItem<T>(T subItemType) where T : Enum
    {
        foreach (var item in gunConfigs)
        {
            if (item.SubItemType.Equals(subItemType))
            {
                return item;
            }
        }
        Debug.LogWarning($"No gun item found with SubItemType: {subItemType}");
        return null;
    }
}

