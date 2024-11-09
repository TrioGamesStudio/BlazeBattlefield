using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunItem : ItemNetworkBase<GunType,GunItemConfig>
{
    protected override void AddToStorage()
    {
        WeaponManager.instance.AddNewGun(config);
    }
}
