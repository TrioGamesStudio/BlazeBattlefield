using Fusion;
using System;
using UnityEngine;

[Serializable]
public class WeaponSlotHandler
{
    
    public GameObject Prefab;
    public GunItemConfig Config;

    public Action OnUpdateNewGunAction;
    public int currentAmmo;

    public Action TurnOn;
    public Action TurnOff;

    public bool IsEmpty => Prefab == null && Config == null;
    public void AddNewWeapon(GunItemConfig newConfig)
    {
        this.Config = newConfig;
        this.Prefab = ItemDatabase.instance.GetItemPrefab(Config.ItemType, Config.SubItemType);
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
        Config = null;
        Prefab = null;
    }
}
