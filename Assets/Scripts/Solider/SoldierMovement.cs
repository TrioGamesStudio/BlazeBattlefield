using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierMovement : NetworkMovementBase
{
    [SerializeField] private NetworkCharacterController controller;

    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private bool isJumpPressed;
    [SerializeField] private Vector3 moveInput;
    [SerializeField] private Transform cameraLook;
    private void Awake()
    {
        controller = GetComponent<NetworkCharacterController>();
    }

    public override void Spawned()
    {
        base.Spawned();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (HasStateAuthority == false) return;

        if (isJumpPressed)
        {
            controller.Jump();
        }
  
        
        if (cameraLook != null)
        {
            Vector3 forward = cameraLook.forward;
            Vector3 right = cameraLook.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            Vector3 desiredMoveDirection = forward * moveInput.z + right * moveInput.x;
            controller.Move(Runner.DeltaTime * speed * desiredMoveDirection);
            
            
            // rotate
            transform.rotation = Quaternion.Euler(0, cameraLook.eulerAngles.y, 0);

        }
    }
    public override void Move(Vector2 inputDirection)
    {
        if(inputDirection == Vector2.zero)
        {
            moveInput = Vector3.zero;
            return;
        }
        moveInput = new Vector3(inputDirection.x, 0, inputDirection.y);
        moveInput.Normalize();
    }

    private void Jump()
    {
        isJumpPressed = true;
    }
    // use when switching movement method: walking to drive ...
    public override void RegisterInput()
    {
        InputPlayerMovement.MoveAction += Move;
        InputPlayerMovement.JumpAction += Jump;
    }

    public override void UnRegisterInput()
    {
        InputPlayerMovement.MoveAction -= Move;
        InputPlayerMovement.JumpAction -= Jump;
        // reset input
        moveInput = Vector3.zero;
    }
}
public abstract class NetworkMovementBase : NetworkBehaviour, ISetupInput, IPlayerMovement
{
    public abstract void Move(Vector2 inputDirection);
    public abstract void RegisterInput();
    public abstract void UnRegisterInput();
}