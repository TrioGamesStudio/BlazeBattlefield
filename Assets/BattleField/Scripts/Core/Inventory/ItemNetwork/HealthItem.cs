using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using NaughtyAttributes;
public class HealthItem : ItemNetworkBase<HealingItemType,HealthItemConfig>
{
    public override void CollectAI(HPHandler hPHandler)
    {
        hPHandler.OnHealRPC(40);
        DestroyItem();
    }

    //public override void Collect()
    //{
    //    StorageManager.instance.UpdateHealth(_enumType, quantity, true);
    //}
    [Button]
    private void TestCollect()
    {
        
    }
}
