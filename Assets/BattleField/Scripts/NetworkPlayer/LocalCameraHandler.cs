using Cinemachine;
using Fusion;
using UnityEngine;

public class LocalCameraHandler : NetworkBehaviour
{
    public Camera localCamera;
    [SerializeField] Transform cameraAnchorPoint; // localCam se di theo camereAnchorPoint
    [SerializeField] Transform spawnedPointGun_OnCam; // nha dan cua sung trong cam
    [SerializeField] Transform spawnedPointGun_OnHand; // nah dan cua sung trong tay player

    //Rotation
    float _cameraRotationX = 0f;
    float _cameraRotationY = 0f;
    Vector2 viewInput;
    Vector2 aim;

    NetworkCharacterController networkCharacterController;

    //Raycast from local camera
    public Vector3 spawnedPointOnCam_Network{get; set;} = Vector3.zero;
    public Vector3 spawnedPointOnHand_Network{get; set;} = Vector3.zero;
    
    public Vector3 hitPoint_Network {get; set;} = Vector3.zero;
    public Vector3 raycastSpawnPointCam_Network {get; set;} = Vector3.zero;
    Ray ray;
    RaycastHit hitInfo;
    [Header("Collisons")]
    [SerializeField] LayerMask collisionLayers;

    [SerializeField] InGameMessagesUIHandler inGameMessagesUIHandler;
    public InGameMessagesUIHandler InGameMessagesUIHandler{get {return inGameMessagesUIHandler;}}

    //others
    [SerializeField] CharacterInputHandler characterInputHandler;
    CinemachineVirtualCamera cinemachineVirtualCamera;
    private void Awake() {
        characterInputHandler = GetComponentInParent<CharacterInputHandler>();

        localCamera = GetComponent<Camera>();
        networkCharacterController = GetComponentInParent<NetworkCharacterController>();
        inGameMessagesUIHandler = GetComponentInChildren<InGameMessagesUIHandler>();
    }

    private void Update() {
        //? view input local
        /* viewInput.x = Input.GetAxis("Mouse X");
        viewInput.y = Input.GetAxis("Mouse Y") * -1f; */

        aim = characterInputHandler.AimDir;
        viewInput.x = aim.x;
        viewInput.y = aim.y * -1f;
    }

    void LateUpdate()
    {
        //? xet cho local cam
        if(cameraAnchorPoint == null) return;
        if(!localCamera.enabled) return;

        if(cinemachineVirtualCamera == null) 
        {
            cinemachineVirtualCamera = FindAnyObjectByType<CinemachineVirtualCamera>();
        }
        else {
            if(characterInputHandler.IsThirdCam)
            {
                if(!cinemachineVirtualCamera.enabled) 
                {
                    cinemachineVirtualCamera.Follow = NetworkPlayer.Local.playerModel;
                    cinemachineVirtualCamera.LookAt = NetworkPlayer.Local.playerModel;
                    cinemachineVirtualCamera.enabled = true;

                    // set playersModel.transform - chuyen sang default Layer - de 3rdPersonCam render thay
                    Utils.SetRenderLayerInChildren(NetworkPlayer.Local.playerModel, LayerMask.NameToLayer("Default"));
                }
                cinemachineVirtualCamera.transform.position = cameraAnchorPoint.position; // localCam di theo | ko phai nam ben trong
                _cameraRotationX += viewInput.y * Time.deltaTime * networkCharacterController.viewRotationSpeed;
                _cameraRotationX = Mathf.Clamp(_cameraRotationX, -90, 90);
                _cameraRotationY += viewInput.x * Time.deltaTime * networkCharacterController.rotationSpeed;

                cinemachineVirtualCamera.transform.rotation = Quaternion.Euler(_cameraRotationX, _cameraRotationY, 0);
                return;
            }
            else 
            {
                if(cinemachineVirtualCamera.enabled) {
                    cinemachineVirtualCamera.enabled = false;

                    //? Set Playerodel - LocalPlayerModel -> de 1stPersomCam render thay
                    Utils.SetRenderLayerInChildren(NetworkPlayer.Local.playerModel, LayerMask.NameToLayer("LocalPlayerModel"));

                }
            }
        }

        localCamera.transform.position = cameraAnchorPoint.position; // localCam di theo | ko phai nam ben trong

        _cameraRotationX += viewInput.y * Time.deltaTime * networkCharacterController.viewRotationSpeed;
        _cameraRotationX = Mathf.Clamp(_cameraRotationX, -90, 90);
        _cameraRotationY += viewInput.x * Time.deltaTime * networkCharacterController.rotationSpeed;

        localCamera.transform.rotation = Quaternion.Euler(_cameraRotationX, _cameraRotationY, 0);
    }

    public void RaycastHitPoint() {
        if(this.Object.HasStateAuthority) {
            ray.origin = this.transform.position;
            ray.direction = this.transform.forward;
            Physics.Raycast(ray, out hitInfo, 100, collisionLayers);
            RPC_SetHitPointRaycast(hitInfo.point, this.transform.position);
            RPC_SetBulletPoint(spawnedPointGun_OnCam.transform.position, spawnedPointGun_OnHand.transform.position);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SetHitPointRaycast(Vector3 hitPointVector, Vector3 raycastSpawnedPoint, RpcInfo info = default) {
        Debug.Log($"[RPC] Set hitPointVector {hitPointVector} for localPlayer");
        this.hitPoint_Network = hitPointVector;
        this.raycastSpawnPointCam_Network = raycastSpawnedPoint;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SetBulletPoint(Vector3 spawnedPointVector, Vector3 spawnedPointVector_,RpcInfo info = default) {
        Debug.Log($"[RPC] Set hitPointVector {spawnedPointVector} for localPlayer");
        this.spawnedPointOnCam_Network = spawnedPointVector;
        this.spawnedPointOnHand_Network = spawnedPointVector_;
    }


}