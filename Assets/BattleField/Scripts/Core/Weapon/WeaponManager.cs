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
    public WeaponHandler weaponHandler;
    public WeaponSlotHandler[] WeaponSlotHandlers { get => weaponSlotHandlers; }

    public event Action<WeaponSlotHandler> OnEquipAction;
    public event Action OnDropAction;

    public AudioClip reloadSound;
    public AudioClip reload_empty_Sound;
    public AudioClip holsterSound;
    public AudioClip un_holsterSound;


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
        SetupSound();
    }
    private void SetupSound()
    {
        reloadSound = SoundManager.SoundAsset.GetSound("reload_15");
        reload_empty_Sound = SoundManager.SoundAsset.GetSound("reload_empty_0");
        holsterSound = SoundManager.SoundAsset.GetSound("holster_0");
        un_holsterSound = SoundManager.SoundAsset.GetSound("un_holster_0");
    }

    private void RegisterEvent()
    {
        InputCombatControl.Instance.Enable();
        InputCombatControl.SwapGun1 += OnActiveWeapon0;
        InputCombatControl.SwapGun2 += OnActiveWeapon1;
        InputCombatControl.SwapGun3 += OnActiveWeapon2;
        InputCombatControl.SwapMeele += OnActiveWeapon3;
        InputCombatControl.Reload += Reload;

    }

    private void OnDestroy()
    {
        InputCombatControl.SwapGun1 -= OnActiveWeapon0;
        InputCombatControl.SwapGun2 -= OnActiveWeapon1;
        InputCombatControl.SwapGun3 -= OnActiveWeapon2;
        InputCombatControl.SwapMeele -= OnActiveWeapon3;
        InputCombatControl.Reload -= Reload;

    }

    private void OnActiveWeapon0() => OnActiveWeapon(0);
    private void OnActiveWeapon1() => OnActiveWeapon(1);
    private void OnActiveWeapon2() => OnActiveWeapon(2);
    private void OnActiveWeapon3() => OnActiveWeapon(3);


    public void Reload()
    {
        if (currentWeaponIndex == -1) return;
        if (weaponSlotHandlers[currentWeaponIndex].TryToReload())
        {
            NetworkPlayer.Local.GetComponent<AudioSource>().CustomPlaySound(reloadSound);
        }
        else
        {
            NetworkPlayer.Local.GetComponent<AudioSource>().CustomPlaySound(reload_empty_Sound);
        }
    }

    public void AddNewGun(GunItemConfig newConfig)
    {
        TimerActionHandler.instance.Cancel();
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
            ChangeCurrentWeaponIndex(newWeaponSlotIndex);
            ShowWeapon(true);
        }

    }

    public void OnActiveWeapon(int activeIndexButton)
    {
        TimerActionHandler.instance.Cancel();
        if (currentWeaponIndex == -1) // T
        {
            // khong cam gi
            if (weaponSlotHandlers[activeIndexButton].IsEmpty) return;
            weaponSlotHandlers[activeIndexButton].Show();
            ChangeCurrentWeaponIndex(activeIndexButton);
            ShowWeapon(true);
        }
        else if (currentWeaponIndex == activeIndexButton)
        {
            // same with weapon onActive
            weaponSlotHandlers[currentWeaponIndex].Hide();
            ChangeCurrentWeaponIndex(-1);
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
                ChangeCurrentWeaponIndex(activeIndexButton);
            }

        }

    }


    public void ShowWeapon(bool v)
    {
        playerAnimator.SetBool("isEquiped", v);
        
    }
    private void ChangeCurrentWeaponIndex(int newIndex)
    {
        currentWeaponIndex = newIndex;
        WeaponUIManager.instance.Hightligh(currentWeaponIndex);

        if(newIndex == -1)
        {
            Debug.Log("Player cat sung");
            NetworkPlayer.Local.GetComponent<AudioSource>().CustomPlaySound(holsterSound);

        }
        else
        {
            Debug.Log("Player cam sung");
            NetworkPlayer.Local.GetComponent<AudioSource>().CustomPlaySound(un_holsterSound);
            weaponHandler.SetConfig(weaponSlotHandlers[currentWeaponIndex].Config);
        }


    }


    public bool IsReadyToShoot()
    {
        if (currentWeaponIndex < 0 || currentWeaponIndex > 4) return false;
        var currentSlot = weaponSlotHandlers[currentWeaponIndex];
        return currentSlot.IsEmpty == false && currentSlot.IsShowInHand;
    }

    public void Shoot()
    {
        TimerActionHandler.instance.Cancel();
        weaponSlotHandlers[currentWeaponIndex].Shoot();
    }

    public bool HasAmmo()
    {
        return weaponSlotHandlers[currentWeaponIndex].HasAmmo;
    }

    public void PlayReloadEmptySound()
    {
        NetworkPlayer.Local.GetComponent<AudioSource>().CustomPlaySound(reload_empty_Sound);
    }
}
