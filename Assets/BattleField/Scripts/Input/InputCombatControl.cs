using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Input/InputCombatControl", fileName = "InputCombatControl", order = 0)]
public class InputCombatControl : InputReader , PlayerInputAction.ICombatActions
{
    public static Action SwapGun1;
    public static Action SwapGun2;
    public static Action SwapGun3;
    public static Action SwapMeele;
    public static Action UseHeal;
    public static Action SwapGrenade;
    public static Action<bool> Attack;
    public static Action Scope;
    public static Action Receive;
    public static Action GetInCar;
    public static Action ChangeFireMode;
    public static Action ShowInventory;
    private void OnEnable()
    {
        Instance.Combat.SetCallbacks(this);
    }
    public void OnSwapGun1(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SwapGun1?.Invoke();
        }
    }

    public void OnSwapGun2(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SwapGun2?.Invoke();
        }
    }

    public void OnSwapGun3(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SwapGun3?.Invoke();
        }
    }

    public void OnSwapMeele(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SwapMeele?.Invoke();
        }
    }

    public void OnUseHeal(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            UseHeal?.Invoke();
        }
    }

    public void OnSwapGrenade(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SwapGrenade?.Invoke();
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Attack?.Invoke(true);
        }else if (context.canceled)
        {
            Attack?.Invoke(false);
        }
    }

    public void OnScope(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Scope?.Invoke();
        }
    }

    public void OnReceive(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Receive?.Invoke();
        }
    }

    public void OnGetInCar(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GetInCar?.Invoke();
        }
    }

    public void OnSwapFireMode(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ChangeFireMode?.Invoke();
        }
    }

    public void OnShowInventory(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ShowInventory?.Invoke();
        }
    }
}