using Fusion;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ammo_Data", menuName = "Item/Ammo_Data")]
public class AmmoItemData : ItemConfigSettings<AmmoType, AmmoItemConfig>
{
    [Button]
    public override void GetDefaultSetting()
    {
        itemConfigs.Clear();
        itemConfigs = new List<AmmoItemConfig>(ItemDefaultConfigs.ammoConfigs);
    }
}
public enum AmmoType
{
    None = 0,
    Ammo556 = 5,
    Ammo762 = 10,
    Ammo9mm = 15,
    Ammo12Gauge = 20
}