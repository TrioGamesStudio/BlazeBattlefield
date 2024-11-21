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
        newWeaponSlotHandler.UIList.Add(this);
        weaponSlotHandler = newWeaponSlotHandler;
        weaponSlotHandler.OnUpdateNewGunUIAction += OnUpdateNewGun;
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
        UpdateTotalAmmo(weaponSlotHandler.TotalAmmo());
        UpdateIcon();
        Debug.Log("Update data:" + weaponSlotHandler.Config.Icon);
    }

    protected virtual void UpdateIcon()
    {
        IconImage.gameObject.SetActive(true);
        IconImage.sprite = weaponSlotHandler.Config.Icon;
    }
  
    public void UpdateTotalAmmo(int totalAmmo)
    {
        totalGunAmmoText.text = totalAmmo.ToString();
    }
    private void Update()
    {
        currentGunAmmoText.text = weaponSlotHandler.currentAmmo.ToString() + "/";
        if (weaponSlotHandler.IsEmpty) return;
        float lerpvalue = (float)weaponSlotHandler.currentAmmo / (float)weaponSlotHandler.Config.maxStack;
        currentGunAmmoText.color = Color.Lerp(Color.red, Color.white, lerpvalue * 1.5f);
    }

    protected virtual void ResetUIState()
    {
        UpdateTotalAmmo(0);
        IconImage.gameObject.SetActive(false);
    }

    
}
