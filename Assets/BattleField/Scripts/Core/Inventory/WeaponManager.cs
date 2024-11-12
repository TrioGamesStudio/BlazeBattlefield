using Fusion.Sockets;
using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;
public class WeaponManager : MonoBehaviour
{
    public static WeaponManager instance;
    
    [SerializeField] private WeaponSlotHandler[] weaponSlotHandlers;
    [SerializeField] private int currentWeaponIndex;
    public int CurrentWeaponIndex { get => currentWeaponIndex; }
    public Animator playerAnimator;
    public ActiveWeapon activeWeapon;
    public WeaponSlotHandler[] WeaponSlotHandlers { get => weaponSlotHandlers; }

    public event Action<WeaponSlotHandler> OnEquipAction;
    public event Action OnDropAction;



    // Need UI to bind with

    private void Awake()
    {
        currentWeaponIndex = -1;
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
        InputCombatControl.Reload += Reload;

    }

    private void OnDestroy()
    {
        InputCombatControl.SwapGun1 -= () => OnActiveWeapon(0);
        InputCombatControl.SwapGun2 -= () => OnActiveWeapon(1);
        InputCombatControl.SwapGun3 -= () => OnActiveWeapon(2);
        InputCombatControl.SwapMeele -= () => OnActiveWeapon(3);
        InputCombatControl.Reload -= Reload;

    }

    public void Reload()
    {
        if (currentWeaponIndex == -1) return;
        weaponSlotHandlers[currentWeaponIndex].TryToReload();
        Debug.Log("Reload ammo");
    }

    public void AddNewGun(GunItemConfig newConfig)
    {
        // truong hop 1
        var newWeaponSlotIndex = (int)newConfig.slotWeaponIndex;
        
        if (IsAllSlotEmpty())
        {
            Debug.LogWarning("truong hop 1");
            var slotOfNewWeapon = weaponSlotHandlers[newWeaponSlotIndex];
            slotOfNewWeapon.AddNewWeapon(newConfig);
            slotOfNewWeapon.Equip();
            slotOfNewWeapon.Show();
            currentWeaponIndex = newWeaponSlotIndex;
            ShowWeapon(true);
            return;
        }

        var currentWeapon = weaponSlotHandlers[currentWeaponIndex];

        if (currentWeapon.Config.slotWeaponIndex == newConfig.slotWeaponIndex) 
        {
            Debug.LogWarning("truong hop 2");
            // truong hop 2: Cung loai, phai bo weapon dang equip, va trang bi vu khi moi
            //activeWeapon.Drop();
            currentWeapon.DeleteAndSpawnWorld();
            currentWeapon.AddNewWeapon(newConfig);
            currentWeapon.Equip();
            currentWeapon.Show();
            //StartCoroutine(Startsdaw(currentWeapon, newConfig));


            //activeWeapon.Equip(currentWeapon);
        }
        else // khong cung index voi curent weapon
        {
            Debug.LogWarning("truong hop 3");
            bool isTargetSlotEmpty = weaponSlotHandlers[newWeaponSlotIndex].IsEmpty;
            if (isTargetSlotEmpty)
            {
                weaponSlotHandlers[newWeaponSlotIndex].AddNewWeapon(newConfig);
                weaponSlotHandlers[newWeaponSlotIndex].Equip();
            }
            else
            {
                // drop 
                var targetSlot = weaponSlotHandlers[newWeaponSlotIndex];
                targetSlot.DeleteAndSpawnWorld();
                targetSlot.AddNewWeapon(newConfig);
                targetSlot.Equip();
            }
        }
    }
    private IEnumerator Startsdaw(WeaponSlotHandler currentWeapon, GunItemConfig newConfig)
    {
        yield return new WaitForSeconds(.2f);
        currentWeapon.AddNewWeapon(newConfig);
        currentWeapon.Equip();
        currentWeapon.Show();
    }
    [Button]
    private void TestDropItem()
    {
        weaponSlotHandlers[currentWeaponIndex].DeleteAndSpawnWorld();
        currentWeaponIndex = -1;
    }
    [Button]
    private void TestHideWeapon()
    {
        weaponSlotHandlers[currentWeaponIndex].Hide();
    }
    [Button]
    private void TestShowWeapon()
    {
        weaponSlotHandlers[currentWeaponIndex].Show();
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


    public bool IsReadyToShoot()
    {
        if (currentWeaponIndex < 0 || currentWeaponIndex > 4) return false;
        return weaponSlotHandlers[currentWeaponIndex].IsEmpty == false && weaponSlotHandlers[currentWeaponIndex].IsShowInHand;
    }
}
