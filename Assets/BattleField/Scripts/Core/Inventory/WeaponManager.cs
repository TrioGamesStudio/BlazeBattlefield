using Fusion;
using System;
using UnityEngine;

public class WeaponManager : NetworkBehaviour
{
    public static WeaponManager instance;
    public WeaponSlotHandler[] WeaponProfilerHolders;
    private WeaponSlotHandler currentSlot;
    public Action OnChangedWeapon;
    public Action OnDiscardCurrentWeapon;
    public int currentWeaponIndex;

    // Need UI to bind with
    private void Awake()
    {
        instance = this;
        // setup for UI
        WeaponProfilerHolders = new WeaponSlotHandler[4];
        WeaponProfilerHolders[0] = new WeaponSlotHandler(GunSlot.MainGun);
        WeaponProfilerHolders[1] = new WeaponSlotHandler(GunSlot.MainGun);
        WeaponProfilerHolders[2] = new WeaponSlotHandler(GunSlot.SubGun);
        WeaponProfilerHolders[3] = new WeaponSlotHandler(GunSlot.Melee);
    }

    public override void Spawned()
    {
        base.Spawned();
    }
    private const int SUBGUN_SLOT_INDEX = 2;
    public void AddNewGun(GunItemConfig newConfig)
    {
        if (newConfig.isSubGun)
        {
            // handle in slot index 3
            // Set new config and prefab
            var subGunWeaponSlot = WeaponProfilerHolders[SUBGUN_SLOT_INDEX];

            if (subGunWeaponSlot.IsEmpty)
            {
                subGunWeaponSlot.Create(newConfig);
                subGunWeaponSlot.TurnOn();
            }
            else
            {
                // Drop and swap to new weapon
                subGunWeaponSlot.TurnOff();
                subGunWeaponSlot.Create(null);
            }


        }
        else
        {
            // handle two first slot

        }
    }
    public void OnCollectMelee()
    {

    }

    public void OnActiveGun(int activeIndexButton)
    {
        if (currentWeaponIndex == activeIndexButton)
        {
            // turn off current slot, because it active same slot
            WeaponProfilerHolders[currentWeaponIndex].TurnOff();
        }
        else
        {
            // if have different weapon, then deActive current, active new one
            if (WeaponProfilerHolders[activeIndexButton].IsEmpty)
            {
                Debug.Log("Ban dang co gang kich hoat 1 slot khong co vu khi", gameObject);
                return;
            }
            WeaponProfilerHolders[currentWeaponIndex].TurnOff();
            WeaponProfilerHolders[activeIndexButton].TurnOn();
            currentWeaponIndex = activeIndexButton;

        }
    }

    public void CreateWeaponItem(NetworkObject prefab, Transform parent, Vector3 position)
    {
        var weapon = Runner.Spawn(prefab, position, Quaternion.identity);
        weapon.transform.SetParent(parent);
        // make sure call this, if not item assume to collect item
        weapon.GetComponent<BoundItem>().AllowAddToCollider = false;
    }

}
