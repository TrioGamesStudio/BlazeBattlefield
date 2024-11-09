using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
public class InventoryUI : MonoBehaviour
{
    // For testing
    
    public WeaponBackpackUI WeaponEquipUI;
    public GameObject view;
    [Header("View UI")]
    [SerializeField] private ViewUIHandler weaponUI;
    [SerializeField] private ViewUIHandler backpackUI;
    [SerializeField] private ViewUIHandler subPanelUI;
    [Header("Shooting")]
    [SerializeField] private WeaponBackpackUI[] weaponEquipUIs;
    private void Awake()
    {
        isOpen = false;
        InputReader.Instance.Enable();
        ShowInventoryElement(false);
        
        
        weaponUI.RaiseSetupUI();
        backpackUI.RaiseSetupUI();
        subPanelUI.RaiseSetupUI();

        InputCombatControl.ShowInventory += ShowInventory;
    }
    private void OnDestroy()
    {
        InputCombatControl.ShowInventory -= ShowInventory;

    }


    private bool isOpen = false;
    private void ShowInventory()
    {
        if (isOpen)
        {
            ShowInventoryElement(true);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            ShowInventoryElement(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        isOpen = !isOpen;
    }
    private void ShowInventoryElement(bool isShow)
    {
        if (isShow)
        {
            weaponUI.Show();
            backpackUI.Show();
            subPanelUI.Show();
        }
        else
        {
            weaponUI.Hide();
            backpackUI.Hide();
            subPanelUI.Hide();
        }
        
    }


    

}