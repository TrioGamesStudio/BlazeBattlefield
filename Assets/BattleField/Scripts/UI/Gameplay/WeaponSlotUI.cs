using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

public class WeaponSlotUI : MonoBehaviour
{
    [SerializeField] private Button ChangeFireModeBtn;
    [SerializeField] private Image GunIcon;
    [SerializeField] private TextMeshProUGUI weaponAmmo;
    [SerializeField] private TextMeshProUGUI totalTypeAmmo;
    [SerializeField] private WeaponSlotHandler WeaponSlotHandler;
    private void Awake()
    {
        SetEmptyText();
    }

    public void BindWeaponSlotHandler(WeaponSlotHandler weaponSlotHandler)
    {
        if(this.WeaponSlotHandler != null)
        {
            WeaponSlotHandler.OnUpdateNewGunAction -= OnUpdateNewGun;
            WeaponSlotHandler.Config.ammoUsingType.OnTotalAmmoChange -= OnTotalAmmoChange;
        }

        WeaponSlotHandler = weaponSlotHandler;
        // listener event from WeaponSlotHandler
        WeaponSlotHandler.OnUpdateNewGunAction += OnUpdateNewGun;
        WeaponSlotHandler.Config.ammoUsingType.OnTotalAmmoChange += OnTotalAmmoChange;

    }


    private void OnUpdateNewGun()
    {
        if (WeaponSlotHandler.IsEmpty)
        {
            SetEmptyText();
        }
        else
        {
            OnCurrentAmmoChange(WeaponSlotHandler.currentAmmo);
            OnTotalAmmoChange(WeaponSlotHandler.Config.ammoUsingType.TotalAmmo);
        }
    }

    private void OnCurrentAmmoChange(int currentAmmo)
    {
        weaponAmmo.text = currentAmmo.ToString() + "/";
    }

    private void OnTotalAmmoChange(int totalAmmo)
    {
        totalTypeAmmo.text = totalAmmo.ToString();
    }

    private void SetEmptyText()
    {
        weaponAmmo.text = "";
        totalTypeAmmo.text = "";
    }
}
