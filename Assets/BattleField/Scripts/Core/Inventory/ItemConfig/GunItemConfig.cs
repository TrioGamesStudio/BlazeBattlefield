using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Item_Data",menuName ="Config/Gun")]
public class GunItemConfig : ItemConfig<GunType>
{
    public float fireRate;
    public int maxRounds;
    public AmmoItemConfig ammoUsingType;
    public SlotWeaponIndex slotWeaponIndex;
    private List<WeaponSlotHandler> weaponListeners;

    public void RemoveNotifyTotalAmmoChange(WeaponSlotHandler weaponSlotHandler)
    {
        if (!weaponListeners.Contains(weaponSlotHandler)) return;
        weaponListeners.Remove(weaponSlotHandler);
        ammoUsingType.OnTotalAmmoChange -= weaponSlotHandler.OnTotalAmmoChange;
    }

    public void AddNotifyTotalAmmoChange(WeaponSlotHandler weaponSlotHandler)
    {
        if (weaponListeners.Contains(weaponSlotHandler)) return;
        weaponListeners.Add(weaponSlotHandler);
        ammoUsingType.OnTotalAmmoChange += weaponSlotHandler.OnTotalAmmoChange;
    }
}

public enum SlotWeaponIndex
{
    Slot_0 = 0,
    Slot_1 = 1,
    Slot_2 = 2,
    Slot_3 = 3,
}