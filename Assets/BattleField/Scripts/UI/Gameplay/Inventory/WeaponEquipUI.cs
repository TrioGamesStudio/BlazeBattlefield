using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class WeaponEquipUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gunName;
    [SerializeField] private TextMeshProUGUI ammoTypeName;
    [SerializeField] private TextMeshProUGUI currentGunAmmo;
    [SerializeField] private TextMeshProUGUI reserveGunAmmo;
    [SerializeField] private Image gunIconImg;
    [SerializeField] private Sprite defaultIcon;
    private WeaponSlotHandler weaponSlothandler;

    private void Awake()
    {
        ResetGunInformation();
    }

    public void BindWeaponData(WeaponSlotHandler weaponSlothandler)
    {
        this.weaponSlothandler = weaponSlothandler;
        weaponSlothandler.OnUpdateNewGunAction += RefreshWeaponInformation;
    }

    private void OnDestroy()
    {
        weaponSlothandler.OnUpdateNewGunAction -= RefreshWeaponInformation;
    }

    public void RefreshWeaponInformation()
    {
        if (weaponSlothandler == null)
        {
            ResetGunInformation();
            return;
        }

        gunName.text = weaponSlothandler.Config.displayName;
        ammoTypeName.text = weaponSlothandler.Config.ammoUsingType.displayName;
        //currentGunAmmo.text = bindWeaponData.currentAmmo+"/";
        //reserveGunAmmo.text = bindWeaponData.totalAmmo;
        //gunIconImg.sprite = bindWeaponData.icon;

    }


    private void ResetGunInformation()
    {
        gunName.text = "";
        ammoTypeName.text = "";
        currentGunAmmo.text = "";
        reserveGunAmmo.text = "";
    }
}
