using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    PlayerInputAction playerInputActions;
    Vector2 move;
    bool isJumped = false;
    bool isFired = false;
    Vector2 aimDir;
    public Vector2 Move{get => move;}
    public bool IsJumped {get => isJumped;}
    public bool IsFired{get => isFired;}
    public Vector2 AimDir {get => aimDir;}
    //others
    

    private void Awake() {
        playerInputActions = new PlayerInputAction();

        playerInputActions.PlayerMovement.Jumping.started += _ => isJumped = true;
        playerInputActions.PlayerMovement.Jumping.canceled += _ => isJumped = false;

        playerInputActions.Combat.Attack.started += _ => isFired = true;
        playerInputActions.Combat.Attack.canceled += _ => isFired = false;
        InputPlayerMovement.LookAction += ChangeLookVector;
    }
    private void ChangeLookVector(Vector2 lookVector)
    {
        Debug.Log("Look vector");
        aimDir = lookVector;
    }
    private void OnEnable() {
        playerInputActions.PlayerMovement.Moving.Enable();
        playerInputActions.PlayerMovement.Jumping.Enable();
        playerInputActions.Combat.Attack.Enable();

        playerInputActions.PlayerMovement.Look.Enable();

    }

    private void OnDisable() {
        playerInputActions.PlayerMovement.Moving.Disable();
        playerInputActions.PlayerMovement.Jumping.Disable();
        playerInputActions.Combat.Attack.Disable();

        playerInputActions.PlayerMovement.Look.Disable();
    }

    private void Update() {
        move = playerInputActions.PlayerMovement.Moving.ReadValue<Vector2>();
        move.Normalize();

        //aimDir = playerInputActions.PlayerMovement.Look.ReadValue<Vector2>();
    }


}
