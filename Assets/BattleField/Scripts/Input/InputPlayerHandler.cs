using Fusion;
using System;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// this class just allow in local player
/// </summary>
public interface ISetupInput
{
    void RegisterInput();
    void UnRegisterInput();
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
    
    
    public void SwitchMovementController(NetworkMovementBase networkMovement)
    {
        movement?.UnRegisterInput();
        movement = networkMovement;
        movement?.RegisterInput();
    }
    public void SwitchCameraController(LocalCameraBase cameraBase)
    {
        firstPersonCamera?.UnRegisterInput();
        firstPersonCamera = cameraBase;
        firstPersonCamera?.RegisterInput();
    }
}
