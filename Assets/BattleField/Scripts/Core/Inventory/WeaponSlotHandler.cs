using Fusion;
using System;
using UnityEngine;

[Serializable]
public class WeaponSlotHandler
{
    
    public NetworkObject Prefab;
    public GunItemConfig Config;

    public Action OnUpdateNewGunAction;

    public bool IsEmpty => Prefab == null && Config == null;
    public void Create(GunItemConfig newConfig)
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

}
