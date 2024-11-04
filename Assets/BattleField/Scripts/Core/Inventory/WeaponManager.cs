using Fusion;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager instance;
    public WeaponSlotHandler[] WeaponSlotHandlers;
    public int currentWeaponIndex;
    public Animator playerAnimator;
    // Need UI to bind with
    private void Start()
    {
        instance = this;
        // setup for UI
        WeaponSlotHandlers = new WeaponSlotHandler[4];
        WeaponSlotHandlers[0] = new WeaponSlotHandler();
        WeaponSlotHandlers[1] = new WeaponSlotHandler();
        WeaponSlotHandlers[2] = new WeaponSlotHandler();
        WeaponSlotHandlers[3] = new WeaponSlotHandler();

        OnInitData?.Invoke(WeaponSlotHandlers);
    }
    private void RegisterEvent()
    {
        InputCombatControl.Instance.Enable();
        InputCombatControl.SwapGun1 += () => OnActiveWeapon(0);
        InputCombatControl.SwapGun2 += () => OnActiveWeapon(1);
        InputCombatControl.SwapGun3 += () => OnActiveWeapon(2);
        InputCombatControl.SwapMeele += () => OnActiveWeapon(3);

    }

  
    private void SwapGun1_performed(InputAction.CallbackContext obj)
    {
        OnActiveWeapon(0);
    }

    private void SwapGun2_performed(InputAction.CallbackContext obj)
    {
        OnActiveWeapon(1);
    }

    private void SwapGun3_performed(InputAction.CallbackContext obj)
    {
        OnActiveWeapon(2);
    }

    private void SwapMeele_performed(InputAction.CallbackContext obj)
    {
        OnActiveWeapon(3);
    }
  

    private const int SUBGUN_SLOT_INDEX = 2;

    public static Action<WeaponSlotHandler[]> OnInitData { get; internal set; }
    public ActiveWeapon activeWeapon;
    public void AddNewGun(GunItemConfig newConfig)
    {
        bool allWeaponIsEmpty = true;
        foreach(var item in WeaponSlotHandlers)
        {
            if (!item.IsEmpty)
            {
                allWeaponIsEmpty = false;
            }
        }
        // truong hop 1
        if (allWeaponIsEmpty)
        {
            var weaponSlot = WeaponSlotHandlers[(int)newConfig.slotWeaponIndex];
            weaponSlot.AddNewWeapon(newConfig);
            
            activeWeapon.Equip(weaponSlot);
            currentWeaponIndex = (int)newConfig.slotWeaponIndex;
            return;
        }
        // truong hop 2
        var currentWeapon = WeaponSlotHandlers[currentWeaponIndex];
        //var equipSlot = WeaponSlotHandlers[(int)newConfig.slotWeaponIndex];
        var index = (int)newConfig.slotWeaponIndex;
        if (currentWeapon.Config.slotWeaponIndex == newConfig.slotWeaponIndex)
        {
            activeWeapon.Drop();
            currentWeapon.AddNewWeapon(newConfig);
            activeWeapon.Equip(currentWeapon);
        }
        else // khong cung slot weapon
        {
            bool isSlotEmpty = WeaponSlotHandlers[index].IsEmpty;
            if (isSlotEmpty)
            {
                WeaponSlotHandlers[index].AddNewWeapon(newConfig);
            }
            else
            {
                // drop 
                WeaponSlotHandlers[index].DeleteAndSpawnWorld();
                WeaponSlotHandlers[index].AddNewWeapon(newConfig);
            }
        }

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
            if (WeaponSlotHandlers[activeIndexButton].IsEmpty)
            {
                Debug.Log("Ban dang co gang kich hoat 1 slot khong co vu khi", gameObject);
                return;
            }
            else
            {
                Debug.Log("Kich hoat weapon moi", gameObject);
                WeaponSlotHandlers[currentWeaponIndex].Hide();
                WeaponSlotHandlers[activeIndexButton].Show();
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
