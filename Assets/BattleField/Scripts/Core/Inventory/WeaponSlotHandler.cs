using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponSlotHandler: IWeaponSlotAction
{
    
    public GameObject Prefab;
    public GunItemConfig Config;

    public Action OnUpdateNewGunAction;

    public int currentAmmo;

    public List<BindingWeaponUI> UIList = new();

    public bool IsEmpty => Prefab == null && Config == null;

    public Action ShowWeaponAction { get; set; }
    public Action HideWeaponAction { get; set; }
    public Action EquipWeaponAction { get; set; }
    public Action DropWeaponAction { get; set; }

    private bool isShowInHand;
    public bool IsShowInHand { get => isShowInHand; }

    public void AddNewWeapon(GunItemConfig newConfig)
    {
        if(newConfig != null)
        {
            Config?.RemoveNotifyTotalAmmoChange(this);
            this.Config = newConfig;
            this.Prefab = ItemDatabase.instance.GetItemPrefab(Config.ItemType, Config.SubItemType);
            Config?.AddNotifyTotalAmmoChange(this);
        }
        else
        {
            this.Config = null;
            this.Prefab = null;
        }
        isShowInHand = false;

        OnUpdateNewGunAction?.Invoke();
    }

    public void OnTotalAmmoChange(int totalAmmo)
    {
        foreach(var item in UIList)
        {
            item.UpdateTotalAmmo(totalAmmo);
        }
    }

    public void OnCurrentAmmoChange()
    {
        foreach (var item in UIList)
        {
            item.UpdateCurrentAmmo(currentAmmo);
        }
    }

    public void Show()
    {
        ShowWeaponAction?.Invoke();
        isShowInHand = true;
    }

    public void Hide()
    {
        HideWeaponAction?.Invoke();
        isShowInHand = false;
    }

    public void Equip()
    {
        if (Prefab == null)
        {
            Debug.LogError("This weapon slot have null prefab");
            return;
        }
        EquipWeaponAction?.Invoke();
    }

    public void DeleteAndSpawnWorld()
    {
        ItemDatabase.instance.GunConfigToWorld(Config, 1);
        AddNewWeapon(null);
        DropWeaponAction?.Invoke();
    }

}
