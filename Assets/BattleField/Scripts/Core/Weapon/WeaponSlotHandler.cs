using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponSlotHandler: IWeaponSlotAction
{
    public WeaponSlotHandler()
    {
        isShowInHand = false;
        StorageManager.OnUpdateItem += () => { OnTotalAmmoChange();} ;
    }

    public GameObject Prefab;
    public GunItemConfig Config;

    public Action OnUpdateNewGunUIAction;

    public int currentAmmo;

    //public List<BindingWeaponUI> UIList = new();




    public bool IsEmpty => Prefab == null && Config == null;
    public bool IsShowInHand { get => isShowInHand; }
    public bool HasAmmo { get => currentAmmo > 0;  }
    public bool isReloading = false;

    [SerializeField] private bool isShowInHand;
    #region FeatureAction
    public Action ShowWeaponAction { get; set; }
    public Action HideWeaponAction { get; set; }
    public Action EquipWeaponAction { get; set; }
    public Action DropWeaponAction { get; set; }

    public Action OnUpdateCurrentAmmo;
    public Action OnUpdateTotalAmmo;
    #endregion Action
    public void AddNewWeapon(GunItemConfig newConfig)
    {
        isReloading = false;
        if (newConfig != null)
        {
            this.Config = newConfig;
            this.Prefab = ItemDatabase.instance.GetItemPrefab(Config.ItemType, Config.SubItemType);
        }
        else
        {
            this.Config = null;
            this.Prefab = null;
        }
        OnUpdateNewGunUIAction?.Invoke();
    }

    public void OnTotalAmmoChange()
    {
        //int totalAmmo = TotalAmmo();
        //foreach(var item in UIList)
        //{
        //    item.UpdateTotalAmmo(totalAmmo);
        //}
        OnUpdateTotalAmmo?.Invoke();
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
        isReloading = false;
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
        TurnAmmoBackWhenDrop();
        ItemDatabase.instance.GunConfigToWorld(Config, 1);
        
        AddNewWeapon(null);
        
        DropWeaponAction?.Invoke();
        currentAmmo = 0;
        OnUpdateCurrentAmmo?.Invoke();
    }

    private void TurnAmmoBackWhenDrop()
    {
        if(currentAmmo > 0)
        {
            var inventory = new InventoryItem();
            inventory.Create(Config.ammoUsingType, currentAmmo);
            StorageManager.instance.Add(ItemType.Ammo, Config.ammoUsingType.SubItemType, inventory);
        }
    }


    public bool TryToReload()
    {
        if (IsEmpty || isShowInHand == false || isReloading) return false;

        int ammoNeed = Config.maxRounds - currentAmmo;

        if (StorageManager.instance.GetTotalAmmo(Config.ammoUsingType.SubItemType) > 0 && ammoNeed > 0)
        {
            isReloading = true;

            TimerActionHandler.instance.StartTimer(1.5f, () =>
            {
                int ammoInStorage = StorageManager.instance.AcquireAmmoItem(Config.ammoUsingType.SubItemType, ammoNeed);

                currentAmmo += ammoInStorage;
                OnUpdateCurrentAmmo?.Invoke();
                isReloading = false;
            },
            () =>
            {
                isReloading = false;
            });
            return true;
        }
        return false;
    }

    public int TotalAmmo()
    {
        if (IsEmpty)
        {
            //Debug.LogError("Slot nay dang bi null, khong the kiem tra tong so dan");
            return 0;
        }

        return StorageManager.instance.GetTotalAmmo(Config.ammoUsingType.SubItemType);
    }

    public void Shoot()
    {
        currentAmmo--;
        OnUpdateCurrentAmmo?.Invoke();
    }
}
