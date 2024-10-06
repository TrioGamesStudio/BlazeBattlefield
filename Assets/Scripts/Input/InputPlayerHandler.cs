using Fusion;
using System;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// this class just allow in local player
/// </summary>
public interface ISetupInput
{
    void RegisterInput(InputPlayerMovement inputPlayerMovement);
    void UnRegisterInput(InputPlayerMovement inputPlayerMovement);
}
public interface IPlayerCamera
{
    void SetTarget(Transform target);
    void Looking(Vector2 direction);
}
public interface IPlayerMovement
{
    void Move(Vector2 inputDirection);
}
public class InputPlayerHandler : NetworkBehaviour
{
    [Header("Reference")]
    [SerializeField] private NetworkMovementBase movement;
    [SerializeField] private LocalCameraBase firstPersonCamera;
    [FormerlySerializedAs("inputReader")] [SerializeField] private InputPlayerMovement inputPlayerMovement;

    public void SetupInput(InputPlayerMovement inputPlayerMovement)
    {
        this.inputPlayerMovement = inputPlayerMovement;
    }
   
    
    public void SwitchMovementController(NetworkMovementBase networkMovement)
    {
        movement?.UnRegisterInput(inputPlayerMovement);
        movement = networkMovement;
        movement?.RegisterInput(inputPlayerMovement);
    }
    public void SwitchCameraController(LocalCameraBase cameraBase)
    {
        firstPersonCamera?.UnRegisterInput(inputPlayerMovement);
        firstPersonCamera = cameraBase;
        firstPersonCamera?.RegisterInput(inputPlayerMovement);
    }
}
