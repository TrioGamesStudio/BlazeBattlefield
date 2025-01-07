using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Input/InputStanceControl", fileName = "InputStanceControl", order = 0)]
public class InputStanceControl : InputReader, PlayerInputAction.IStanceControlActions
{
    public static Action ProneAction;
    public static Action CrouchAction;
    public static Action StandAction;

    public override void SetCallbacks()
    {
        Instance.StanceControl.SetCallbacks(this);
    }

    public void OnProne(InputAction.CallbackContext context)
    {
        ProneAction?.Invoke();
    }

    public void OnStand(InputAction.CallbackContext context)
    {
        CrouchAction?.Invoke();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        StandAction?.Invoke();
    }

}