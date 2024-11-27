using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Item_Data",menuName ="Config/Gun")]
public class GunItemConfig : ItemConfig<GunType>
{
    [Header("Gun Settings 1")]
    public Sprite IconActualGun;
    public AmmoItemConfig ammoUsingType;
    public SlotWeaponIndex slotWeaponIndex;
    [Header("Gun Settings 2")]
    public bool isSingleMode = true;
    public bool isContainScope = false;
    public float cooldownTime = 0.5f;
    public byte damagePerHit = 1;
    public int maxRounds;
    [Header("Audio")]
    public AudioClip shootingSound;
    public AudioClip reloadSound;
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