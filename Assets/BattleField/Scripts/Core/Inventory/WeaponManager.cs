using Fusion;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
public class WeaponManager : MonoBehaviour
{
    public static WeaponManager instance;
    
    [SerializeField] private WeaponSlotHandler[] weaponSlotHandlers;
    [SerializeField] private int currentWeaponIndex;
    
    public Animator playerAnimator;
    public ActiveWeapon activeWeapon;
    public WeaponSlotHandler[] WeaponSlotHandlers { get => weaponSlotHandlers; }

    public event Action<WeaponSlotHandler> OnEquipAction;
    public event Action OnDropAction;



    // Need UI to bind with

    private void Awake()
    {
        instance = this;
        weaponSlotHandlers = new WeaponSlotHandler[4];
        weaponSlotHandlers[0] = new WeaponSlotHandler();
        weaponSlotHandlers[1] = new WeaponSlotHandler();
        weaponSlotHandlers[2] = new WeaponSlotHandler();
        weaponSlotHandlers[3] = new WeaponSlotHandler();
        RegisterEvent();
    }


    private void RegisterEvent()
    {
        InputCombatControl.Instance.Enable();
        InputCombatControl.SwapGun1 += () => OnActiveWeapon(0);
        InputCombatControl.SwapGun2 += () => OnActiveWeapon(1);
        InputCombatControl.SwapGun3 += () => OnActiveWeapon(2);
        InputCombatControl.SwapMeele += () => OnActiveWeapon(3);

    }

    private void OnDestroy()
    {
        InputCombatControl.SwapGun1 -= () => OnActiveWeapon(0);
        InputCombatControl.SwapGun2 -= () => OnActiveWeapon(1);
        InputCombatControl.SwapGun3 -= () => OnActiveWeapon(2);
        InputCombatControl.SwapMeele -= () => OnActiveWeapon(3);
    }

    public void AddNewGun(GunItemConfig newConfig)
    {
        // truong hop 1
        var newWeaponSlotIndex = (int)newConfig.slotWeaponIndex;
        var currentWeapon = weaponSlotHandlers[currentWeaponIndex];
        
        if (IsAllSlotEmpty())
        {
            Debug.LogWarning("truong hop 1");
            var slotOfNewWeapon = weaponSlotHandlers[newWeaponSlotIndex];
            slotOfNewWeapon.AddNewWeapon(newConfig);
            slotOfNewWeapon.Equip();
            currentWeaponIndex = newWeaponSlotIndex;
            ShowWeapon(true);
        }
        else if (currentWeapon.Config.slotWeaponIndex == newConfig.slotWeaponIndex) 
        {
            Debug.LogWarning("truong hop 2");
            // truong hop 2: Cung loai, phai bo weapon dang equip, va trang bi vu khi moi
            //activeWeapon.Drop();
            currentWeapon.DeleteAndSpawnWorld();
            currentWeapon.AddNewWeapon(newConfig);
            currentWeapon.Equip();
            //activeWeapon.Equip(currentWeapon);
        }
        else // khong cung slot weapon
        {
            Debug.LogWarning("truong hop 3");
            bool isSlotEmpty = weaponSlotHandlers[newWeaponSlotIndex].IsEmpty;
            if (isSlotEmpty)
            {
                weaponSlotHandlers[newWeaponSlotIndex].AddNewWeapon(newConfig);
            }
            else
            {
                // drop 
                weaponSlotHandlers[newWeaponSlotIndex].DeleteAndSpawnWorld();
                weaponSlotHandlers[newWeaponSlotIndex].AddNewWeapon(newConfig);
            }
        }

    }

    [Button]
    private void TestDropItem()
    {
        weaponSlotHandlers[0].DeleteAndSpawnWorld();
    }
    [Button]
    private void TestHideWeapon()
    {
        weaponSlotHandlers[0].Hide();
    }
    [Button]
    private void TestShowWeapon()
    {
        weaponSlotHandlers[0].Show();
    }



    private bool IsAllSlotEmpty()
    {
        bool allWeaponIsEmpty = true;
        foreach (var item in weaponSlotHandlers)
        {
            if (!item.IsEmpty)
            {
                allWeaponIsEmpty = false;
            }
        }

        return allWeaponIsEmpty;
    }

    public void OnActiveWeapon(int activeIndexButton)
    {
        if(currentWeaponIndex == -1) // T
        {
            // khong cam gi
            if (weaponSlotHandlers[activeIndexButton].IsEmpty) return;
            weaponSlotHandlers[activeIndexButton].Show();
            currentWeaponIndex = activeIndexButton;
            ShowWeapon(true);
        }
        else if (currentWeaponIndex == activeIndexButton)
        {
            // same with weapon onActive
            weaponSlotHandlers[currentWeaponIndex].Hide();
            currentWeaponIndex = -1;
            ShowWeapon(false);
            // turn off current slot, because it active same slot
        }
        else
        {
            // if have 2 different weapon, then deActive current, active new one
            if (weaponSlotHandlers[activeIndexButton].IsEmpty)
            {
                Debug.Log("Ban dang co gang kich hoat 1 slot khong co vu khi", gameObject);
                return;
            }
            else
            {
                Debug.Log("Kich hoat weapon moi", gameObject);
                weaponSlotHandlers[currentWeaponIndex].Hide();
                weaponSlotHandlers[activeIndexButton].Show();
                currentWeaponIndex = activeIndexButton;
            }

        }
    }


    public void ShowWeapon(bool v)
    {
        playerAnimator.SetBool("isEquiped", v);
    }
}
