using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
public class InventoryUI : MonoBehaviour
{
    // For testing
    
    public WeaponEquipUI WeaponEquipUI;
    public GameObject view;

    private void Awake()
    {
        // WeaponEquipUI.BindWeaponData(Gun1);
        // WeaponEquipUI.RefreshWeaponInformation();
        InputReader.Instance.Enable();
        view.gameObject.SetActive(false);
        InputCombatControl.ShowInventory += ShowInventory;
    }

    private void OnDestroy()
    {
        InputCombatControl.ShowInventory -= ShowInventory;
    }

    private void ShowInventory()
    {
        view.SetActive(!view.activeSelf);
    }
    
    [Button]
    public void Test()
    {
        WeaponEquipUI.RefreshWeaponInformation();
    }
}