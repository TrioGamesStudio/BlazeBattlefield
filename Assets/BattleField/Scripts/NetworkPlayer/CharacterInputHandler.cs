using Unity.VisualScripting;
using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    ActiveWeapon ActiveWeapon;
    PlayerInputAction playerInputActions;
    CharacterMovementHandler characterMovementHandler;
    WeaponHandler weaponHandler;
    Vector2 move;
    bool isJumped = false;
    bool isFired = false;
    Vector2 aimDir;
    public Vector2 Move{get => move;}
    public bool IsJumped {get => isJumped;}
    public bool IsFired{get => isFired;}
    public Vector2 AimDir {get => aimDir;}
    // check run button
    //[SerializeField] bool isSprintPressed;

    public bool isPrinted = false;
    //others
    [SerializeField] bool isThirdCam = false;
    public bool IsThirdCam{get => isThirdCam;}

    public bool IsZoomed = false;

    bool isChatVoice = false;
    public bool IsChatVoice {get => isChatVoice;}
    
    private void Awake() {
        weaponHandler = GetComponent<WeaponHandler>();
        ActiveWeapon = GetComponent<ActiveWeapon>();
        characterMovementHandler = GetComponent<CharacterMovementHandler>();
        playerInputActions = new PlayerInputAction();

        playerInputActions.PlayerMovement.Jumping.started += _ => isJumped = true;
        playerInputActions.PlayerMovement.Jumping.canceled += _ => isJumped = false;

        playerInputActions.Combat.Attack.started += _ => weaponHandler.SetFireInput(true);
        playerInputActions.Combat.Attack.canceled += _ => weaponHandler.SetFireInput(false);

        playerInputActions.Combat.Scope.started += _ => weaponHandler.SetZoomInput(true);
        playerInputActions.Combat.Scope.canceled += _ => weaponHandler.SetZoomInput(false);


        /* playerInputActions.PlayerMovement.SwitchCam.started += _ => isSwitchCam = true;
        playerInputActions.PlayerMovement.SwitchCam.canceled += _ => isSwitchCam = false; */

        playerInputActions.PlayerMovement.SwitchCam.started += _ => ChangeCamera();
        
        InputPlayerMovement.LookAction += ChangeLookVector;

        playerInputActions.PlayerMovement.Sprint.started += _ => UpdateIsPrinted();

        playerInputActions.Combat.ChatVoice.started += _ => isChatVoice = true;
        playerInputActions.Combat.ChatVoice.canceled += _ => isChatVoice = false;

    }

    private void Start() {
        isPrinted = false;
    }

    void UpdateIsPrinted() {
        isPrinted = !isPrinted;
        if(!isPrinted) characterMovementHandler.SetMovementInput(false);
        else characterMovementHandler.SetMovementInput(true);
    }

    private void ChangeCamera()
    {
        isThirdCam = !isThirdCam;
        ActiveWeapon.SetActiveLocalWeapon(!isThirdCam);
    }

    private void ChangeLookVector(Vector2 lookVector)
    {
        //Debug.Log("Look vector");
        if (InventoryUI.instance.IsOpen)
        {
            aimDir = Vector3.zero;
            return;
        }
        aimDir = lookVector;
    }
    private void OnEnable() {
        playerInputActions.PlayerMovement.Moving.Enable();
        playerInputActions.PlayerMovement.Jumping.Enable();
        playerInputActions.Combat.Attack.Enable();
        playerInputActions.Combat.Scope.Enable();
        playerInputActions.Combat.ChatVoice.Enable();




        playerInputActions.PlayerMovement.Look.Enable();

        playerInputActions.PlayerMovement.SwitchCam.Enable();
        playerInputActions.PlayerMovement.Sprint.Enable();
    }

    private void OnDisable() {
        playerInputActions.PlayerMovement.Moving.Disable();
        playerInputActions.PlayerMovement.Jumping.Disable();
        playerInputActions.Combat.Attack.Disable();
        playerInputActions.Combat.Scope.Disable();
        playerInputActions.Combat.ChatVoice.Disable();



        playerInputActions.PlayerMovement.Look.Disable();

        playerInputActions.PlayerMovement.SwitchCam.Disable();
        playerInputActions.PlayerMovement.Sprint.Disable();
    }

    private void Update() {
        move = playerInputActions.PlayerMovement.Moving.ReadValue<Vector2>();
        move.Normalize();
        
        if(move.magnitude > 0.1f && isPrinted) {
            isPrinted = !isPrinted;
            characterMovementHandler.SetMovementInput(false);
        } 

        //aimDir = playerInputActions.PlayerMovement.Look.ReadValue<Vector2>();

        /* if(isThirdCam && isFired) {
            isThirdCam = false;
            ActiveWeapon.SetActiveLocalWeapon(true);
        } */
    }


}
