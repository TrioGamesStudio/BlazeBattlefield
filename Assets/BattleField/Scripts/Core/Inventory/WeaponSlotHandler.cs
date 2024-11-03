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

    public Action<GunItemConfig> OnUpdateNewGunAction;

    public bool IsEmpty => Prefab == null && Config == null;

    public void Create(GunItemConfig newConfig)
    {
        this.Config = newConfig;
        this.Prefab = ItemDatabase.instance.GetItemPrefab(Config.ItemType, Config.SubItemType);
        OnUpdateNewGunAction?.Invoke(Config);
    }

    public WeaponSlotHandler(GunSlot slot)
    {
        Slot = slot;
    }


}
