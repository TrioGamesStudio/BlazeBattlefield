using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class HealthItem : ItemNetworkBase
{
    public HealingItemType HealingItemType;
    public override void Collect()
    {
        StorageManager.instance.AddHealth(HealingItemType, quantity);
        DestroyRPC();
    }
}
