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
    private WeaponInformation bindWeaponData;
    
    [Serializable]
    public class WeaponInformation
    {
        public string weaponName;
        public string ammoType;
        public string currentAmmo;
        public string totalAmmo;
        public Sprite icon;
    }

    public void BindWeaponData(WeaponInformation currentWeapon)
    {
        this.bindWeaponData = currentWeapon;
    }
    public void RefreshWeaponInformation()
    {
        if (bindWeaponData == null)
        {
            ResetGunInformation();
            return;
        }
        
        gunName.text = bindWeaponData.weaponName;
        ammoTypeName.text = bindWeaponData.ammoType;
        currentGunAmmo.text = bindWeaponData.currentAmmo+"/";
        reserveGunAmmo.text = bindWeaponData.totalAmmo;
        gunIconImg.sprite = bindWeaponData.icon;

    }

    private void ResetGunInformation()
    {
        gunName.text = "";
        ammoTypeName.text = "";
        currentGunAmmo.text = "";
        reserveGunAmmo.text = "";
    }
}
