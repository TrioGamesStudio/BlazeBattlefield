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
    
    private const int SUBGUN_SLOT_INDEX = 2;

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

    }


    private void RegisterEvent()
    {
        InputCombatControl.Instance.Enable();
        InputCombatControl.SwapGun1 += () => OnActiveWeapon(0);
        InputCombatControl.SwapGun2 += () => OnActiveWeapon(1);
        InputCombatControl.SwapGun3 += () => OnActiveWeapon(2);
        InputCombatControl.SwapMeele += () => OnActiveWeapon(3);

    }

    public void AddNewGun(GunItemConfig newConfig)
    {
        // truong hop 1
        var newWeaponSlotIndex = (int)newConfig.slotWeaponIndex;
        var currentWeapon = weaponSlotHandlers[currentWeaponIndex];
        if (IsAllSlotEmpty())
        {
            AddNewWeaponToSlot(newConfig, newWeaponSlotIndex);
        }
        else if (currentWeapon.Config.slotWeaponIndex == newConfig.slotWeaponIndex) 
        {
            // truong hop 2: Cung loai, phai bo weapon dang equip, va trang bi vu khi moi
            //activeWeapon.Drop();
            currentWeapon.DeleteAndSpawnWorld();
            currentWeapon.AddNewWeapon(newConfig);
            currentWeapon.Equip();
            //activeWeapon.Equip(currentWeapon);
        }
        else // khong cung slot weapon
        {
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

    private void AddNewWeaponToSlot(GunItemConfig newConfig, int newWeaponSlotIndex)
    {
        var slotOfNewWeapon = weaponSlotHandlers[newWeaponSlotIndex];
        slotOfNewWeapon.AddNewWeapon(newConfig);
        //activeWeapon.Equip(slotOfNewWeapon);
        slotOfNewWeapon.Equip();
        currentWeaponIndex = newWeaponSlotIndex;
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
        }

        if (currentWeaponIndex == activeIndexButton)
        {
            // same with weapon onActive
            ShowWeapon(false);
            currentWeaponIndex = -1;
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

    //public void CreateWeaponItem(NetworkObject prefab, Transform parent, Vector3 position)
    //{
    //    var weapon = Runner.Spawn(prefab, position, Quaternion.identity);
    //    weapon.transform.SetParent(parent);
    //    // make sure call this, if not item assume to collect item
    //    weapon.GetComponent<BoundItem>().AllowAddToCollider = false;
    //}

    public void ShowWeapon(bool v)
    {
        playerAnimator.SetBool("isEquiped", v);
    }
}
