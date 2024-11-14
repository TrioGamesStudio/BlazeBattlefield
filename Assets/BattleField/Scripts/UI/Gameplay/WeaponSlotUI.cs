using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;
public class BindingWeaponUI : MonoBehaviour
{
    [SerializeField] protected WeaponSlotHandler weaponSlotHandler;
    [SerializeField] protected SlotWeaponIndex weaponIndex;
    [SerializeField] protected TextMeshProUGUI currentGunAmmoText;
    [SerializeField] protected TextMeshProUGUI totalGunAmmoText;
    [SerializeField] protected Image IconImage;
    public SlotWeaponIndex WeaponIndex { get => weaponIndex; }


    public virtual void BindWeaponSlot(WeaponSlotHandler newWeaponSlotHandler)
    {
        newWeaponSlotHandler.UIList.Add(this);
        weaponSlotHandler = newWeaponSlotHandler;
        weaponSlotHandler.OnUpdateNewGunAction += OnUpdateNewGun;
        OnUpdateNewGun();
    }

    private void OnUpdateNewGun()
    {
        if (weaponSlotHandler.IsEmpty)
        {
            // remove callback UI of ammo
            Debug.Log("Reset UI callback", gameObject);
            ResetUIState();
        }
        else
        {
            // add ammocallback
            UpdateGunInfor();
        }
    }
    protected virtual void UpdateGunInfor()
    {
        //Debug.Log("IsEmpty: " + weaponSlotHandler.IsEmpty);
        UpdateCurrentAmmo(weaponSlotHandler.currentAmmo);
        UpdateTotalAmmo(weaponSlotHandler.TotalAmmo());
        IconImage.gameObject.SetActive(true);
        IconImage.sprite = weaponSlotHandler.Config.Icon;
        Debug.Log("Update data:" + weaponSlotHandler.Config.Icon);
    }

    public void UpdateCurrentAmmo(int currentAmmo)
    {
        currentGunAmmoText.text = currentAmmo.ToString() + "/";
    }

    public void UpdateTotalAmmo(int totalAmmo)
    {
        totalGunAmmoText.text = totalAmmo.ToString();
    }

    protected virtual void ResetUIState()
    {
        UpdateCurrentAmmo(0);
        UpdateTotalAmmo(0);
        IconImage.gameObject.SetActive(false);
    }
}
public class WeaponSlotUI : BindingWeaponUI
{

}
