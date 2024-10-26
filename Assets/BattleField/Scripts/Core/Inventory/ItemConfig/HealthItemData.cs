using System;
using UnityEngine;

[CreateAssetMenu(fileName ="Health_Data",menuName ="Item/Health_Data")]
public class HealthItemData : ItemConfigSettings<HealingItemType, HealthItemConfig>
{

}
[Serializable]
public class HealthItemConfig : ItemConfig<HealingItemType>
{
    public int healthAmount;
    public float usingTime;
}
