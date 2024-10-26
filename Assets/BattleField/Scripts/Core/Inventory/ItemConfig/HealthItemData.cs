using System;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
[CreateAssetMenu(fileName ="Health_Data",menuName ="Item/Health_Data")]
public class HealthItemData : ItemConfigSettings<HealingItemType, HealthItemConfig>
{
    //public override HealthItemConfig GetItemDataConfig(HealingItemType _EnumType)
    //{
    //    return ExtensionHelper.GetUIItem(itemConfigs, _EnumType);
    //}
}
[Serializable]
public class HealthItemConfig : ItemConfig<HealingItemType>
{
    public int healthAmount;
    public float usingTime;
}
