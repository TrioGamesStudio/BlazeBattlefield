using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    ActiveWeapon ActiveWeapon;
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
    [SerializeField] bool isThirdCam = false;
    public bool IsThirdCam{get => isThirdCam;}

    private void Awake() {
        ActiveWeapon = GetComponent<ActiveWeapon>();
        playerInputActions = new PlayerInputAction();

        playerInputActions.PlayerMovement.Jumping.started += _ => isJumped = true;
        playerInputActions.PlayerMovement.Jumping.canceled += _ => isJumped = false;

        playerInputActions.Combat.Attack.started += _ => isFired = true;
        playerInputActions.Combat.Attack.canceled += _ => isFired = false;

        /* playerInputActions.PlayerMovement.SwitchCam.started += _ => isSwitchCam = true;
        playerInputActions.PlayerMovement.SwitchCam.canceled += _ => isSwitchCam = false; */

        playerInputActions.PlayerMovement.SwitchCam.started += _ => ChangeCamera();
        
        InputPlayerMovement.LookAction += ChangeLookVector;
    }

    private void ChangeCamera()
    {
        isThirdCam = !isThirdCam;
        ActiveWeapon.SetActiveLocalWeapon(!isThirdCam);
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

        playerInputActions.PlayerMovement.SwitchCam.Enable();
    }

    private void OnDisable() {
        playerInputActions.PlayerMovement.Moving.Disable();
        playerInputActions.PlayerMovement.Jumping.Disable();
        playerInputActions.Combat.Attack.Disable();

        playerInputActions.PlayerMovement.Look.Disable();

        playerInputActions.PlayerMovement.SwitchCam.Disable();

    }

    private void Update() {
        move = playerInputActions.PlayerMovement.Moving.ReadValue<Vector2>();
        move.Normalize();

        //aimDir = playerInputActions.PlayerMovement.Look.ReadValue<Vector2>();

        if(isThirdCam && isFired) {
            isThirdCam = false;
            ActiveWeapon.SetActiveLocalWeapon(true);
        }
        //aimDir = playerInputActions.PlayerMovement.Look.ReadValue<Vector2>();
    }


}
