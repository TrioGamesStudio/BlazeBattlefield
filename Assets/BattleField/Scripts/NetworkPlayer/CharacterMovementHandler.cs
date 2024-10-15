using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class CharacterMovementHandler : NetworkBehaviour
{
    // other
    NetworkCharacterController networkCharacterController;
    LocalCameraHandler localCameraHandler;

    //input
    private bool isJumped = false;
    bool isJumping = false;

    Vector2 movementInput;
    Vector3 aimForwardVector;

    //locomotion
    [SerializeField] float walkSpeed = 0f;
    [SerializeField] Animator anim;

    // request after falling
    [SerializeField] float fallHightToRespawn = -10f;
    [SerializeField] bool isRespawnRequested = false;

    [Networked]
    public bool isRespawnRequested_{get; set;} = false;

    //...
    NetworkInGameMessages networkInGameMessages;
    NetworkPlayer networkPlayer;

    HPHandler hPHandler;
    CharacterInputHandler characterInputHandler;

    private void Awake() {
        characterInputHandler = GetComponent<CharacterInputHandler>();
        networkCharacterController = GetComponent<NetworkCharacterController>();
        localCameraHandler = GetComponentInChildren<LocalCameraHandler>();
        networkInGameMessages = GetComponent<NetworkInGameMessages>();
        networkPlayer = GetComponent<NetworkPlayer>();
        anim = GetComponentInChildren<Animator>();
        hPHandler = GetComponent<HPHandler>();
    }


    void Update() {
        //lock input to move and jump if Ready scene
        if(SceneManager.GetActiveScene().name == "Ready") return;

        //? move input local
        /* if (Input.GetButtonDown("Jump")) _jumpPressed = true;
        movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")); */
        movementInput = characterInputHandler.Move;
        isJumped = characterInputHandler.IsJumped;

        aimForwardVector = localCameraHandler.transform.forward;
    }
    
    public override void FixedUpdateNetwork() {
        if (HasStateAuthority == false) return;

        // ko chay doan duoi neu dang fall or respawn
        if(Object.HasStateAuthority) {
            if(isRespawnRequested_) {
                Respawn();
                return;
            }

            // ko cap nhat vi tri movement khi player death
            if(hPHandler.Networked_IsDead) return;
        }

        Move(movementInput);
        Jump();

        CheckFallToRespawn();
    }

    void Move(Vector2 movementInput) {
        transform.forward = aimForwardVector;

        // khong cho xoay player len xuong quanh x
        Quaternion rotation = transform.rotation;
        rotation.eulerAngles = new Vector3(0f, rotation.eulerAngles.y, rotation.eulerAngles.z);
        transform.rotation = rotation;

        //move network
        Vector3 moveDir = transform.forward * movementInput.y + transform.right * movementInput.x;
        moveDir.Normalize();

        networkCharacterController.Move(moveDir);

        // animator
        Vector2 walkVector = new Vector2(networkCharacterController.Velocity.x,
                                        networkCharacterController.Velocity.z);
        
        walkVector.Normalize(); // ko cho lon hon 1
        walkSpeed = Mathf.Lerp(walkSpeed, Mathf.Clamp01(walkVector.magnitude), Runner.DeltaTime * 10f);

        anim.SetFloat("walkSpeed", walkSpeed);
    }

    void Jump() {
        if(!isJumping && isJumped) {
            isJumping = true;
            StartCoroutine(JumpCO(0.5f));
        }
    }
    IEnumerator JumpCO(float time) {
        networkCharacterController.Jump();
        yield return new WaitForSeconds(time);
        isJumping = false;
    }

    private void CheckFallToRespawn() {
        if(transform.position.y < fallHightToRespawn) {
            if(Object.HasStateAuthority) {
                Debug.Log($"{Time.time} respawn due to fall {transform.position}");

                //? thong bao khi fall out
                networkInGameMessages.SendInGameRPCMessage(networkPlayer.nickName_Network.ToString(), " -> fall off");
                Respawn();
            }
        }
    }

    public void CharacterControllerEnable(bool isEnable) {
        networkCharacterController.enabled = isEnable;
    }

    private void Respawn() {
        Debug.Log($"_____Starting Respawn");
        CharacterControllerEnable(true);

        networkCharacterController.Teleport(Utils.GetRandomSpawnPoint());
        
        hPHandler.OnRespawned_ResetHPIsDead(); // khoi tao lai gia tri HP isDeath - false
        ////isRespawnRequested = false;
        RPC_SetNetworkedIsDead(false);
        Debug.Log($"_____Ending Respawn");

    }
    
    public void RequestRespawn() {
        Debug.Log($"_____Requested Respawn");
        /* isRespawnRequested = true; */
        RPC_SetNetworkedIsDead(true);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SetNetworkedIsDead(bool isRespawnRequested) {
        this.isRespawnRequested_ = isRespawnRequested;
    }

}