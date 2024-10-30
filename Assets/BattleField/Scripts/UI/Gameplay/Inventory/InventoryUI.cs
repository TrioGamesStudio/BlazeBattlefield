using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
public class InventoryUI : MonoBehaviour
{
    // For testing
    
    public WeaponEquipUI WeaponEquipUI;
    public GameObject view;
    [Header("View UI")]
    [SerializeField] private ViewUIHandler weaponUI;
    [SerializeField] private ViewUIHandler backpackUI;
    [SerializeField] private ViewUIHandler subPanelUI;
    private void Awake()
    {
        // WeaponEquipUI.BindWeaponData(Gun1);
        // WeaponEquipUI.RefreshWeaponInformation();
        InputReader.Instance.Enable();
        isOpen = false;
        CloseAll();
        InputCombatControl.ShowInventory += ShowInventory;
        weaponUI.RaiseSetupUI();
        backpackUI.RaiseSetupUI();
        subPanelUI.RaiseSetupUI();


    }
    private void OnDestroy()
    {
        InputCombatControl.ShowInventory -= ShowInventory;
    }

    [Button]
    private void CloseAll()
    {
        weaponUI.Hide();
        backpackUI.Hide();
        subPanelUI.Hide();
    }

    private bool isOpen = false;
    private void ShowInventory()
    {
        if (isOpen)
        {
            weaponUI.Show();
            backpackUI.Show();
            subPanelUI.Show();
        }
        else
        {
            CloseAll();
        }
        isOpen = !isOpen;
    }


}