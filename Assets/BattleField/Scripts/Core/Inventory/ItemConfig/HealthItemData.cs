using NaughtyAttributes;
using System;
using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "Health_Data", menuName = "Item/Health_Data")]
public class HealthItemData : ItemConfigSettings<HealingItemType, HealthItemConfig>
{
    [Button]
    public override void GetDefaultSetting()
    {
        itemConfigs.Clear();
        itemConfigs = new List<HealthItemConfig>(ItemDefaultConfigs.healthConfigs);
    }
}
public enum LevelType
{
    Level_1,
    Level_2,
    Level_3
}
[CreateAssetMenu(fileName = "Ammor_Data", menuName = "Item/Health_Data")]
public class AmmorItemConfig : ItemConfig<LevelType>
{
    public float damageReduction;
    public float durability;
}
public class HelmetItemConfig : ItemConfig<LevelType>
{
    public float damageReduction;
    public float durability;
}

public class BackpackItemConfig : ItemConfig<LevelType>
{
    public float capacity;
}