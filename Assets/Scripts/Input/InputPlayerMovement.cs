using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
[CreateAssetMenu(fileName = "InputPlayerMovement", menuName = "Input/InputPlayerMovement")]
public class InputPlayerMovement : InputReader, PlayerInputAction.IPlayerMovementActions
{

    public Action<Vector2> MoveAction;
    public Action<Vector2> LookAction;
    public Action JumpAction;
    private void OnEnable()
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
        Debug.Log("Moving: " + context.ReadValue<Vector2>());
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            LookAction?.Invoke(context.ReadValue<Vector2>());
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            LookAction?.Invoke(Vector2.zero);
        }
        Debug.Log("Looking: " + context.ReadValue<Vector2>());
    }

    public void OnJumping(InputAction.CallbackContext context)
    {
        JumpAction?.Invoke();
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
}