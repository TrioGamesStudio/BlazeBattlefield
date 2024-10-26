using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoItem : ItemNetworkBase
{
    public AmmoType ammoType;


    public override void Collect()
    {
        StorageManager.instance.UpdateAmmo(ammoType, quantity,true);
    }

}
