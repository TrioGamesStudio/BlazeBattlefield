using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Item_Data",menuName ="Config/Gun")]
public class GunItemConfig : ItemConfig<GunType>
{
    public bool isSingleMode = true;
    public float cooldownTime = 0.5f;
    public int maxRounds;
    public AmmoItemConfig ammoUsingType;
    public SlotWeaponIndex slotWeaponIndex;
    private bool isInitialize = false;
    public Sprite IconActualGun;
    public AudioClip shootingSound;
    public AudioClip reloadSound;
    public byte damagePerHit = 1;
    public RecoilGunSettings recoil;
}

public enum SlotWeaponIndex
{
    Slot_0 = 0,
    Slot_1 = 1,
    Slot_2 = 2,
    Slot_3 = 3,
}
[Serializable]
public class RecoilGunSettings
{
    [Header("Recoil")]
    public float currentRecoilX = -2;
    public float currentRecoilY = 2;
    public float currentRecoilZ = 0.35f;
    public float currentReturnSpeed = 2;
    public float currentSnappiness = 6;
}