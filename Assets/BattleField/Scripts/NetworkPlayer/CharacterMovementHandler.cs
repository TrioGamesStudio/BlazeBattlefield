using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using ExitGames.Client.Photon.StructWrapping;
using Unity.VisualScripting;

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
    bool isSprinted;
    //locomotion

    Animator anim;
    int speedAnimRate = 1;
    [SerializeField] float currentSpeedAnim = 0;

    // request after falling
    [SerializeField] float fallHightToRespawn = -10f;
    //[SerializeField] bool isRespawnRequested = false;

    [Networked]
    public bool isRespawnRequested_{get; set;} = false;

    //...
    NetworkInGameMessages networkInGameMessages;
    NetworkPlayer networkPlayer;
    HPHandler hPHandler;
    CharacterInputHandler characterInputHandler;
    Vector3 moveDir;
    const int maxSpeedWalk = 5;
    const int maxSpeedSprint = 6;

    [SerializeField] AudioSource audioSource;
    bool isPlaySound = false;

    [Header("Recoil")]
    [SerializeField] float recoilX;
    [SerializeField] float recoilY;
    [SerializeField] float recoilZ;
    [SerializeField] float snappiness_;
    [SerializeField] float returnSpeed_;

    private void Awake() {
        //audioSource = GetComponent<AudioSource>();
        characterInputHandler = GetComponent<CharacterInputHandler>();
        networkCharacterController = GetComponent<NetworkCharacterController>();
        localCameraHandler = GetComponentInChildren<LocalCameraHandler>();
        networkInGameMessages = GetComponent<NetworkInGameMessages>();
        networkPlayer = GetComponent<NetworkPlayer>();
        anim = GetComponentInChildren<Animator>();
        hPHandler = GetComponent<HPHandler>();
        speedAnimRate = 1;

    }

    private void Start() {
        audioSource.clip = SoundManager.SoundAsset.GetSound("walk_slow_0");
    }


    void Update() {
        //lock input to move and jump if Ready scene
        if(SceneManager.GetActiveScene().name == "MainLobby") return;

        //? move input local
        /* if (Input.GetButtonDown("Jump")) _jumpPressed = true;
        movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")); */
        
        if(!isSprinted) {
            movementInput = characterInputHandler.Move;
            //networkCharacterController.maxSpeed = 4;
        }
        else {

            movementInput = Vector2.up;
            //networkCharacterController.maxSpeed = 6;
        }
        
        isJumped = characterInputHandler.IsJumped;

        aimForwardVector = localCameraHandler.transform.forward;
    }
    
    public override void FixedUpdateNetwork() {
        if (HasStateAuthority == false) return;
        if (SceneManager.GetActiveScene().name == "MainLobby") return;
        // ko chay doan duoi neu dang fall or respawn
        if (Object.HasStateAuthority) {
            if(isRespawnRequested_) {
                //! test cho nhung scene ben ngoai -> khong can check IsDone
                //if(SceneManager.GetActiveScene().name == "Quang_Scene") {
                //    Respawn();
                //    return;
                //}

                //if(Matchmaking.Instance.IsDone || MatchmakingTeam.Instance.IsDone) {
                //    RespawnOnStartingBattle();
                //    return;
                //}
                //else {
                //    Respawn();
                //    return;
                //}
                
            }

            // ko cap nhat vi tri movement khi player death
            if(hPHandler.Networked_IsDead) return;
        }

        Move(movementInput);

        Jump();

        CheckFallToRespawn();
    }

    public void SetMovementInput(bool isSprinted) {
        this.isSprinted = isSprinted;
        if(isSprinted) {
            networkCharacterController.maxSpeed = maxSpeedSprint;
            speedAnimRate = 4;
        }
        else {
            networkCharacterController.maxSpeed = maxSpeedWalk;
            speedAnimRate = 1;
        }
    }

    void Move(Vector2 movementInput) {
        transform.forward = aimForwardVector;

        // khong cho xoay player len xuong quanh x
        Quaternion rotation = transform.rotation;
        rotation.eulerAngles = new Vector3(0f, rotation.eulerAngles.y, rotation.eulerAngles.z);
        transform.rotation = rotation;

        //move network
        moveDir = transform.forward * movementInput.y + transform.right * movementInput.x;

        moveDir.Normalize();
        networkCharacterController.Move(moveDir);
        
        // shaking camera
        if(moveDir.magnitude > 0.1f) {
            localCameraHandler.SetRecoil_(recoilX, recoilY, recoilZ, returnSpeed_, snappiness_);
        }
        // animator
        Vector2 walkVector = new Vector2(networkCharacterController.Velocity.x,
                                        networkCharacterController.Velocity.z);
        
        walkVector.Normalize(); // ko cho lon hon 1

        currentSpeedAnim = Mathf.Lerp(currentSpeedAnim, Mathf.Clamp01(walkVector.magnitude), Runner.DeltaTime * 10f);
        anim.SetFloat("walkSpeed", currentSpeedAnim * speedAnimRate);

        // sound
        if(moveDir.magnitude > 0.1 && !isPlaySound) {
            isPlaySound = true;
            audioSource.Play();
        }
        else if(moveDir.magnitude < 0.1) {
            audioSource.Stop();
            isPlaySound = false;
        } 
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
                GetComponent<PlayerMessageManager>().FallOffLogRPC($"{networkPlayer.nickName_Network.ToString()}");
                Respawn();
            }
        }
    }

    public void CharacterControllerEnable(bool isEnable) {
        networkCharacterController.enabled = isEnable;
    }
    [EditorButton]
    public void Respawn() {
        Debug.Log($"_____Starting Respawn");
        CharacterControllerEnable(true);

        networkCharacterController.Teleport(Utils.GetRandomSpawnPointOnWaitingArea());
        
        hPHandler.OnRespawned_ResetHPIsDead(); // khoi tao lai gia tri HP isDeath - false
        ////isRespawnRequested = false;
        RPC_SetNetworkedIsDead(false);
        Debug.Log($"_____Ending Respawn");

    }
    [EditorButton]
    public void RespawnOnStartingBattle() {
        Debug.Log($"_____ random spawn before starting battle");
        CharacterControllerEnable(true);
        networkCharacterController.Teleport(Utils.GetRandomSpawnPointOnStartingBattle());
        RPC_SetNetworkedIsDead(false);
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