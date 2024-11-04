using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

public class WeaponSlotUI : MonoBehaviour
{
    public Button ChangeFireModeBtn;
    public Image GunIcon;
    public TextMeshProUGUI weaponAmmo;
    public TextMeshProUGUI totalTypeAmmo;
    public WeaponSlotHandler WeaponSlotHandler;


    public void BindWeaponSlotHandler(WeaponSlotHandler weaponSlotHandler)
    {
        WeaponSlotHandler = weaponSlotHandler;
        // listener event from WeaponSlotHandler
        WeaponSlotHandler.OnUpdateNewGunAction += OnUpdateNewGun;
    }
    private void OnDestroy()
    {
        WeaponSlotHandler.OnUpdateNewGunAction -= OnUpdateNewGun;

    }

    private void OnUpdateNewGun()
    {
    }
    private void OnCurrentAmmoChange(int currentAmmo)
    {
        weaponAmmo.text = currentAmmo.ToString() + "/";
    }
    private void OnTotalAmmoChange(int totalAmmo)
    {
        totalTypeAmmo.text = totalAmmo.ToString();
    }
}
