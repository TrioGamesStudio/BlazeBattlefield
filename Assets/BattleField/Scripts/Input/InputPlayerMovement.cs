using System;
using UnityEngine;
using UnityEngine.InputSystem;
[CreateAssetMenu(fileName = "InputPlayerMovement", menuName = "Input/InputPlayerMovement")]
public class InputPlayerMovement : InputReader, PlayerInputAction.IPlayerMovementActions
{
    public static Action<Vector2> MoveAction;
    public static Action<Vector2> LookAction;
    public static Action JumpAction;
    public static Action SwitchCamAction;
    public static Action SprintAction;



    //private void OnEnable()
    public override void SetCallbacks()
    {
        Instance.PlayerMovement.SetCallbacks(this);
    }
    public void OnMoving(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            MoveAction?.Invoke(context.ReadValue<Vector2>());
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            MoveAction?.Invoke(Vector2.zero);
        }
        // Debug.Log("Moving: " + context.ReadValue<Vector2>());
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        LookAction?.Invoke(context.ReadValue<Vector2>());
        // if (context.phase == InputActionPhase.Performed)
        // {
        //     LookAction?.Invoke(context.ReadValue<Vector2>());
        // }
        // else if (context.phase == InputActionPhase.Canceled)
        // {
        //     LookAction?.Invoke(Vector2.zero);
        // }
        // Debug.Log("Looking: " + context.ReadValue<Vector2>());
    }

    public void OnJumping(InputAction.CallbackContext context)
    {
        JumpAction?.Invoke();
    }

    public void OnSwitchCam(InputAction.CallbackContext context) {
        SwitchCamAction?.Invoke();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        SprintAction?.Invoke();
    }

}

public abstract class InputReader : ScriptableObject
{
    private static PlayerInputAction instance;
    public static PlayerInputAction Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PlayerInputAction();
            }
            return instance;
        }
    }
    public abstract void SetCallbacks();

}