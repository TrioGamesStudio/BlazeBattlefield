using Cinemachine;
using Fusion;
using UnityEngine;
using UnityEngine.UIElements;

public class LocalCameraHandler : NetworkBehaviour
{
    public Camera localCamera;
    [SerializeField] Transform cameraAnchorPoint; // localCam se di theo camereAnchorPoint
    [SerializeField] Transform spawnedPointGun_OnCam; // nha dan cua sung trong cam
    [SerializeField] Transform spawnedPointGun_OnHand; // nah dan cua sung trong tay player

    //Rotation
    [SerializeField] private int angleLimit = 70;
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

    #region Recoil
    public Vector3 currentRotation;
    public Vector3 targetRotation;

    [Header("Recoil")]
    [SerializeField] float recoilX;
    [SerializeField] float recoilY;
    [SerializeField] float recoilZ;
    [SerializeField] float snappiness;
    [SerializeField] float returnSpeed;
    Animator animator;
    #endregion Recoil
    

    #region Shaking Camera
    public Vector3 currentRotation_;
    public Vector3 targetRotation_;
    [SerializeField] float snappiness_;
    [SerializeField] float returnSpeed_;
    #endregion Shaking Camera

    private void Awake() {
        characterInputHandler = GetComponentInParent<CharacterInputHandler>();

        localCamera = GetComponent<Camera>();
        networkCharacterController = GetComponentInParent<NetworkCharacterController>();
        inGameMessagesUIHandler = GetComponentInChildren<InGameMessagesUIHandler>();

        animator = GetComponentInChildren<Animator>();
    }

    private void Update() {
        //? view input local
        /* viewInput.x = Input.GetAxis("Mouse X");
        viewInput.y = Input.GetAxis("Mouse Y") * -1f; */

        aim = characterInputHandler.AimDir;
        viewInput.x = aim.x;
        viewInput.y = aim.y * -1f;

        RecoilUpdate();

        // neu ko di chuyen thi ko shaking
        RecoilUpdate_();// shaking
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
                _cameraRotationX = Mathf.Clamp(_cameraRotationX, -angleLimit, angleLimit);
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
        _cameraRotationX = Mathf.Clamp(_cameraRotationX, -angleLimit, angleLimit);
        _cameraRotationY += viewInput.x * Time.deltaTime * networkCharacterController.rotationSpeed;

        //localCamera.transform.rotation = Quaternion.Euler(_cameraRotationX, _cameraRotationY, 0);
        localCamera.transform.rotation = Quaternion.Euler(new Vector3(_cameraRotationX, _cameraRotationY, 0) + currentRotation + currentRotation_);
    }
    public Vector3 accuryVector = Vector3.one;
    public void RaycastHitPoint() {
        if(this.Object.HasStateAuthority) {
            ray.origin = this.transform.position;
            ray.direction = this.transform.forward;
            var direction = ray.direction;
            direction.x += Random.Range(-accuryVector.x, accuryVector.x);
            direction.y += Random.Range(-accuryVector.y, accuryVector.y);
            direction.z += Random.Range(-accuryVector.z, accuryVector.z);
            ray.direction = direction;
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


    void RecoilUpdate() {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        //targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);
    }
    void SetRecoil(float recoilX, float recoilY, float recoilZ, float returnSpeed, float snappiness) {
        animator.SetTrigger("trigger");
        this.returnSpeed = returnSpeed;
        this.snappiness = snappiness;
        targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));

    }

    public void SetRecoil(RecoilGunSettings recoil)
    {
        SetRecoil(recoil.currentRecoilX, recoil.currentRecoilY, recoil.currentRecoilZ, recoil.currentReturnSpeed, recoil.currentSnappiness);
    }

    // Shaking camera when player moving
    void RecoilUpdate_() {
        targetRotation_ = Vector3.Lerp(targetRotation_, Vector3.zero, returnSpeed_ * Time.deltaTime);
        currentRotation_ = Vector3.Slerp(currentRotation_, targetRotation_, snappiness_ * Time.fixedDeltaTime);
    }
    public void SetRecoil_(float recoilX, float recoilY, float recoilZ, float returnSpeed, float snappiness) {
        this.returnSpeed_ = returnSpeed;
        this.snappiness_ = snappiness;
        targetRotation_ += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));

    }
}