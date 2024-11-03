using Fusion;
using System;
using UnityEngine;
using static Fusion.Allocator;
public enum GunSlot
{
    MainGun,
    SubGun,
    Melee
}
[Serializable]
public class WeaponSlotHandler
{
    public GunSlot Slot;
    
    public NetworkObject Prefab;
    public GunItemConfig Config;

    public Action OnTurnOff;
    public Action OnTurnOn;

    public Action<GunItemConfig> OnUpdateNewGun;

    public bool IsEmpty => Prefab == null && Config == null;

    public void Create(GunItemConfig newConfig)
    {
        this.Config = newConfig;
        Prefab = ItemDatabase.instance.GetItemPrefab(Config.ItemType, Config.SubItemType);
        OnUpdateNewGun?.Invoke(Config);
    }

    public WeaponSlotHandler(GunSlot slot)
    {
        Slot = slot;
    }

    public void UpdateGun(GunItemConfig newConfig)
    {
        Config = newConfig;
        Prefab = ItemDatabase.instance.GetItemPrefab(Config.ItemType, Config.SubItemType);
    }

    public void TurnOn()
    {
        Debug.Log("Turn On Gun Slot");
        OnTurnOn?.Invoke();
    }

    public void TurnOff()
    {
        Debug.Log("Turn off Gun Slot");
        OnTurnOff?.Invoke();
    }

    public void Delete()
    {
        TurnOff();
        ItemDatabase.instance.GunConfigToWorld(Config,1);
        Prefab = null;
        Config = null;
        OnUpdateNewGun?.Invoke(null);
    }
}
