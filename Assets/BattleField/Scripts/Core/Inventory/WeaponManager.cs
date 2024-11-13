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
        int newWeaponSlotIndex = (int)newConfig.slotWeaponIndex;
        bool allSlotIsEmpty = true;
        bool isEquipWeapon = currentWeaponIndex != -1;

        foreach(var item in weaponSlotHandlers)
        {
            if(item.IsEmpty == false)
            {
                allSlotIsEmpty = false;
                break;
            }
        }

        if (!weaponSlotHandlers[newWeaponSlotIndex].IsEmpty)
        {
            weaponSlotHandlers[newWeaponSlotIndex].DeleteAndSpawnWorld();
        }

        weaponSlotHandlers[newWeaponSlotIndex].AddNewWeapon(newConfig);
        weaponSlotHandlers[newWeaponSlotIndex].Equip();

        if (!isEquipWeapon && allSlotIsEmpty)
        {
            weaponSlotHandlers[newWeaponSlotIndex].Hide();
            weaponSlotHandlers[newWeaponSlotIndex].Show();
            currentWeaponIndex = newWeaponSlotIndex;
            ShowWeapon(true);
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
