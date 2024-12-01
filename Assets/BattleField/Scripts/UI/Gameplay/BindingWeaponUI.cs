using DG.Tweening;
using TMPro;
using UnityEngine;
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
        //newWeaponSlotHandler.UIList.Add(this);
        weaponSlotHandler = newWeaponSlotHandler;
        weaponSlotHandler.OnUpdateNewGunUIAction += OnUpdateNewGun;
        weaponSlotHandler.OnUpdateCurrentAmmo += () => UpdateCurrentAmmo(weaponSlotHandler.currentAmmo);
        weaponSlotHandler.OnUpdateTotalAmmo += () => UpdateTotalAmmo(weaponSlotHandler.TotalAmmo());
        OnUpdateNewGun();
    }
    

    private void OnUpdateNewGun()
    {
        if (weaponSlotHandler.IsEmpty)
        {
            // remove callback UI of ammo
            Debug.Log("Reset UI callback", gameObject);
            ResetToDefaultState();
        }
        else
        {
            // add ammocallback
            UpdateNewGunInformation();
        }
    }
    protected virtual void UpdateNewGunInformation()
    {
        //Debug.Log("IsEmpty: " + weaponSlotHandler.IsEmpty);
        UpdateTotalAmmo(weaponSlotHandler.TotalAmmo());
        UpdateCurrentAmmo(weaponSlotHandler.currentAmmo);
        UpdateIcon();
        Debug.Log("Update data:" + weaponSlotHandler.Config.Icon);
    }
    protected virtual void ResetToDefaultState()
    {
        UpdateTotalAmmo(0);
        UpdateCurrentAmmo(0);
        IconImage.gameObject.SetActive(false);
    }
    protected virtual void UpdateIcon()
    {
        IconImage.gameObject.SetActive(true);
        IconImage.sprite = weaponSlotHandler.Config.Icon;
    }
    protected virtual void UpdateTotalAmmo(int totalAmmo)
    {
        totalGunAmmoText.text = totalAmmo.ToString();
    }
    protected virtual void UpdateCurrentAmmo(int currentAmmo)
    {
        Debug.Log("Set text current ammo: " + currentAmmo);
        currentGunAmmoText.text = currentAmmo.ToString();
    }
}
