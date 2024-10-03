using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
[CreateAssetMenu(fileName = "Input Reader",menuName = "Input/Input Reader")]
public class InputReader : ScriptableObject, PlayerInputAction.IPlayerActions
{
    private PlayerInputAction InputAction;

    public Action<Vector2> MoveAction;
    public Action<Vector2> LookAction;
    public Action JumpAction;
    private void OnEnable()
    {
        if (InputAction == null)
        {
            InputAction = new PlayerInputAction();
            InputAction.Player.SetCallbacks(this);
        }
    }
    public void EnableInput()
    {
        InputAction.Enable();
    }

    public void OnMoving(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            MoveAction?.Invoke(context.ReadValue<Vector2>());
        }else if(context.phase == InputActionPhase.Canceled)
        {
            MoveAction?.Invoke(Vector2.zero);
        }
        Debug.Log("Moving: "+context.ReadValue<Vector2>());
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        LookAction?.Invoke(context.ReadValue<Vector2>());
        Debug.Log("Looking: "+context.ReadValue<Vector2>());
    }
}
