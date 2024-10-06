using Fusion;
using System;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// this class just allow in local player
/// </summary>
public interface ISetupInput
{
    void RegisterInput(InputReader inputReader);
    void UnRegisterInput(InputReader inputReader);
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
    [FormerlySerializedAs("inputPlayerMovement")] [SerializeField] private InputReader inputReader;

    public void SetupInput(InputReader inputReader)
    {
        this.inputReader = inputReader;
    }
   
    
    public void SwitchMovementController(NetworkMovementBase networkMovement)
    {
        movement?.UnRegisterInput(inputReader);
        movement = networkMovement;
        movement?.RegisterInput(inputReader);
    }
    public void SwitchCameraController(LocalCameraBase cameraBase)
    {
        firstPersonCamera?.UnRegisterInput(inputReader);
        firstPersonCamera = cameraBase;
        firstPersonCamera?.RegisterInput(inputReader);
    }
}
