using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunItem : ItemNetworkBase<GunType,GunItemConfig>
{
    public override void CollectAI(ActiveWeaponAI activeWeaponAI)
    {
        Debug.Log("...AI collect " + gameObject.name);
        activeWeaponAI.Equip(ItemDatabase.instance.GetItemPrefab(config.ItemType, config.SubItemType));
        DestroyItem();
    }

    //public override void CollectAI(ActiveWeaponAI activeWeaponAI)
    //{
    //    Debug.Log("...AI collect " + gameObject.name);
    //    activeWeaponAI.Equip(ItemDatabase.instance.GetItemPrefab(config.ItemType, config.SubItemType));
    //    DestroyItem();
    //}

    protected override void AddToStorage()
    {
        WeaponManager.instance.AddNewGun(config);
    }
}
