using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class HealthItem : ItemNetworkBase<HealingItemType,HealthItemConfig>
{
    public override void Collect()
    {
        StorageManager.instance.UpdateHealth(_enumType, quantity, true);
    }
}
