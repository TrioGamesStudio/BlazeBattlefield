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
    private bool isInitialize = false;
    public Sprite IconActualGun;
    public AudioClip shootingSound;
    public AudioClip reloadSound;
    public float damagePerHit = 1;
}

public enum SlotWeaponIndex
{
    Slot_0 = 0,
    Slot_1 = 1,
    Slot_2 = 2,
    Slot_3 = 3,
}