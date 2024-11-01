using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using NaughtyAttributes;
public class HealthItem : ItemNetworkBase<HealingItemType,HealthItemConfig>
{
    //public override void Collect()
    //{
    //    StorageManager.instance.UpdateHealth(_enumType, quantity, true);
    //}
    [Button]
    private void TestCollect()
    {
        
    }
}
