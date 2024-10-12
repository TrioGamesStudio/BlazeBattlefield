using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;
public class CharacterMovementHandler : NetworkBehaviour
{
    // other
    NetworkCharacterController networkCharacterController;
    LocalCameraHandler localCameraHandler;

    //input
    private bool _jumpPressed;
    Vector3 aimForwardVector;
    Vector2 movementInput;

    // request after falling
    [SerializeField] float fallHightToRespawn = -10f;
    [SerializeField] bool isRespawnRequested = false;

    [Networked]
    public bool isRespawnRequested_{get; set;} = false;

    //...
    NetworkInGameMessages networkInGameMessages;
    NetworkPlayer networkPlayer;
    //HPHandler hPHandler;
    private void Awake() {
        networkCharacterController = GetComponent<NetworkCharacterController>();
        localCameraHandler = GetComponentInChildren<LocalCameraHandler>();
        networkInGameMessages = GetComponent<NetworkInGameMessages>();
        networkPlayer = GetComponent<NetworkPlayer>();
        //hPHandler = GetComponent<HPHandler>();
    }


    void Update() {
        //lock input to move and jump if Ready scene
        if(SceneManager.GetActiveScene().name == "Ready") return;

        //? move input local
        if (Input.GetButtonDown("Jump")) _jumpPressed = true;
        movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        aimForwardVector = localCameraHandler.transform.forward;
    }
    
    public override void FixedUpdateNetwork() {
        // Only move own player and not every other player. Each player controls its own player object.
        if (HasStateAuthority == false) return;

        // ko chay doan duoi neu dang fall or respawn
        if(Object.HasStateAuthority) {
            if(isRespawnRequested_) {
                Respawn();
                return;
            }
            // ko cap nhat vi tri movement khi player death
            //if(hPHandler.Networked_IsDead) return; 
        }

        //xoay local player theo aimForwardVector -> dam bao localPlayer nhin thang se la huong aimForwardVector
        transform.forward = aimForwardVector;

        // khong cho xoay player len xuong quanh x
        Quaternion rotation = transform.rotation;
        rotation.eulerAngles = new Vector3(0f, rotation.eulerAngles.y, rotation.eulerAngles.z);
        transform.rotation = rotation;

        //move network
        Vector3 moveDir = transform.forward * movementInput.y + transform.right * movementInput.x;
        moveDir.Normalize();

        networkCharacterController.Move(moveDir);

        //jump network
        if(_jumpPressed) {
            networkCharacterController.Jump();
            _jumpPressed = !_jumpPressed;
        }

        // animator


        
        CheckFallToRespawn();
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
        
        //hPHandler.OnRespawned_ResetHPIsDead(); // khoi tao lai gia tri HP isDeath - false
        ////isRespawnRequested = false;
        RPC_SetNetworkedIsDead(false);
        Debug.Log($"_____Ending Respawn");

    }
    
    public void RequestRespawn() {
        Debug.Log($"_____Requested Respawn");
        //isRespawnRequested = true;
        RPC_SetNetworkedIsDead(true);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SetNetworkedIsDead(bool isRespawnRequested) {
        this.isRespawnRequested_ = isRespawnRequested;
    }
}