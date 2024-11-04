using Fusion;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : NetworkBehaviour
{
    public static WeaponManager instance;
    public WeaponSlotHandler[] WeaponProfilerHolders;
    public int currentWeaponIndex;
    public Animator playerAnimator;
    // Need UI to bind with
    private void Start()
    {
        instance = this;
        // setup for UI
        WeaponProfilerHolders = new WeaponSlotHandler[4];
        WeaponProfilerHolders[0] = new WeaponSlotHandler();
        WeaponProfilerHolders[1] = new WeaponSlotHandler();
        WeaponProfilerHolders[2] = new WeaponSlotHandler();
        WeaponProfilerHolders[3] = new WeaponSlotHandler();

        OnInitData?.Invoke(WeaponProfilerHolders);
    }
    PlayerInputAction playerInputActions;
    private void RegisterEvent()
    {
        playerInputActions = new PlayerInputAction();
        playerInputActions.Combat.Enable();
        playerInputActions.Combat.SwapGun1.performed += SwapGun1_performed;
        playerInputActions.Combat.SwapGun2.performed += SwapGun2_performed;
        playerInputActions.Combat.SwapGun3.performed += SwapGun3_performed;
        playerInputActions.Combat.SwapMeele.performed += SwapMeele_performed;
    }

    private void OnDestroy()
    {
        playerInputActions.Combat.SwapGun1.performed -= SwapGun1_performed;
        playerInputActions.Combat.SwapGun2.performed -= SwapGun2_performed;
        playerInputActions.Combat.SwapGun3.performed -= SwapGun3_performed;
        playerInputActions.Combat.SwapMeele.performed -= SwapMeele_performed;
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
  
    public override void Spawned()
    {
        base.Spawned();
    }
    private const int SUBGUN_SLOT_INDEX = 2;

    public static Action<WeaponSlotHandler[]> OnInitData { get; internal set; }

    public void AddNewGun(GunItemConfig newConfig)
    {
        if (newConfig.isSubGun)
        {
            Debug.Log("Collect item");
            // handle in slot index 3
            // Set new config and prefab
            var subGunWeaponSlot = WeaponProfilerHolders[SUBGUN_SLOT_INDEX];

            if (subGunWeaponSlot.IsEmpty)
            {
                Debug.Log("Create slot");
                subGunWeaponSlot.Create(newConfig);
                OnActiveWeapon(SUBGUN_SLOT_INDEX);
            }
        }
        else
        {
            // handle two first slot

        }
    }
    private void Update()
    {
        for (int i = 0; i < WeaponProfilerHolders.Length; i++)
        {
            Debug.Log($"Index: {i},Prefab: {WeaponProfilerHolders[i].Prefab}, Config:{WeaponProfilerHolders[i].Config}");
        }
    }

    public void OnActiveWeapon(int activeIndexButton)
    {

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
            if (WeaponProfilerHolders[activeIndexButton].IsEmpty)
            {
                Debug.Log("Ban dang co gang kich hoat 1 slot khong co vu khi", gameObject);
                return;
            }
            else
            {
                Debug.Log("Kich hoat weapon moi", gameObject);
                WeaponProfilerHolders[currentWeaponIndex].Hide();
                WeaponProfilerHolders[activeIndexButton].Show();
                currentWeaponIndex = activeIndexButton;
            }

        }
    }

    public void CreateWeaponItem(NetworkObject prefab, Transform parent, Vector3 position)
    {
        var weapon = Runner.Spawn(prefab, position, Quaternion.identity);
        weapon.transform.SetParent(parent);
        // make sure call this, if not item assume to collect item
        weapon.GetComponent<BoundItem>().AllowAddToCollider = false;
    }

    public void ShowWeapon(bool v)
    {
        playerAnimator.SetBool("isEquiped", v);
    }
}
