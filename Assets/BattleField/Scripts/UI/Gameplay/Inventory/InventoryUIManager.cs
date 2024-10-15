using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
public class InventoryUIManager : MonoBehaviour
{
    // For testing
    public WeaponEquipUI.WeaponInformation Gun1;
    public WeaponEquipUI.WeaponInformation Gun2;
    public WeaponEquipUI.WeaponInformation subGun;
    public WeaponEquipUI.WeaponInformation Melee;
    
    public WeaponEquipUI WeaponEquipUI;
   
    private void Awake()
    {
        WeaponEquipUI.BindWeaponData(Gun1);
        WeaponEquipUI.RefreshWeaponInformation();
    }
    [Button]
    public void Test()
    {
        WeaponEquipUI.RefreshWeaponInformation();
    }
}
