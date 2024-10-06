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
        // Moving
        //transform.Translate(Runner.DeltaTime * speed * moveInput);

        if (cameraLook != null)
            transform.rotation = Quaternion.Euler(0, cameraLook.eulerAngles.y, 0);
        
        
        controller.Move(Runner.DeltaTime * speed * moveInput);
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
    public void Jump()
    {
        isJumpPressed = true;
    }
    public override void RegisterInput(InputPlayerMovement inputPlayerMovement)
    {
        inputPlayerMovement.MoveAction += Move;
        inputPlayerMovement.JumpAction += Jump;
    }

    public override void UnRegisterInput(InputPlayerMovement inputPlayerMovement)
    {
        inputPlayerMovement.MoveAction -= Move;
        inputPlayerMovement.JumpAction -= Jump;
        // reset input
        moveInput = Vector3.zero;
    }
}
public abstract class NetworkMovementBase : NetworkBehaviour, ISetupInput, IPlayerMovement
{
    public abstract void Move(Vector2 inputDirection);
    public abstract void RegisterInput(InputPlayerMovement inputPlayerMovement);
    public abstract void UnRegisterInput(InputPlayerMovement inputPlayerMovement);
}