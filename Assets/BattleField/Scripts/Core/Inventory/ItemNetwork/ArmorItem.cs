using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class ArmorItem : ItemNetworkBase<ArmorType, ArmorConfig>
{
    [SerializeField] private GameObject pickupModel;
    [SerializeField] private GameObject equipModel;

    public override void CollectAI(ActiveWeaponAI activeWeaponAI)
    {
        DestroyItem();
    }

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