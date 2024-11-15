using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class ArmorItem : ItemNetworkBase<ArmorType, ArmorConfig>
{
    protected override void AddToStorage()
    {
        GearManager.instance.AddArmor(config, customDatas[0].value);
    }
    [EditorButton]
    private void InitCustomData()
    {
        customDatas = new CustomData[1] { new CustomData(GearManager.DurabilityKey, config.durability) };
    }
}