using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponSlotHandler
{
    
    public GameObject Prefab;
    public GunItemConfig Config;

    public Action OnUpdateNewGunAction;
    public Action<int> OnAmmoChange;

    public int currentAmmo;
    public bool IsEmpty => Prefab == null && Config == null;
    public void AddNewWeapon(GunItemConfig newConfig)
    {
        if(newConfig != null)
        {
            this.Config = newConfig;
            this.Prefab = ItemDatabase.instance.GetItemPrefab(Config.ItemType, Config.SubItemType);
        }
        else
        {
            this.Config = null;
            this.Prefab = null;
        }
        
        OnUpdateNewGunAction?.Invoke();
    }

    public void Show()
    {
        Debug.Log("show weapon");
    }

    public void Hide()
    {
        Debug.Log("Hide weapon");
    }

    public void DeleteAndSpawnWorld()
    {
        ItemDatabase.instance.GunConfigToWorld(Config, 1);
        AddNewWeapon(null);
    }
}
