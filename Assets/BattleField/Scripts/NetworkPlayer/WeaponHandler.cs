using System.Collections;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponHandler : NetworkBehaviour, INetworkInitialize
{
    [SerializeField] BulletHandler bulletVFXPF; // vien dan chua class BulletHandler (chi co chua hieu ung vxf tai noi raycast hit vao)
    [Header("Effects")]
    [SerializeField] ParticleSystem fireParticleSystemLocal;
    [SerializeField] ParticleSystem fireParticleSystemRemote;

    [Header("Aim")]
    [SerializeField] Transform aimPoint; // VI TRI LOCAL CAMERA 1st and 3rd
    [SerializeField] Transform aimPoint_grandeRocket; // VI TRI TREN NONG SUNG trong 1stPersonCam
    [SerializeField] Transform aimPoint_grandeRocket_3rd; // VI TRI TREN NONG SUNG trong 1stPersonCam

    [Header("Collisons")]
    [SerializeField] LayerMask collisionLayers;

    [Header("Sound")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip weaponSoundCurr;


    [Networked] // bien updated through the server on all the clients
    public bool isFiring { get; set; }
    ChangeDetector changeDetector;

    float lastTimeFired = 0f;

    //timing cho fire Grenade
    TickTimer grenadeFireDelay = TickTimer.None;
    TickTimer rocketFireDelay = TickTimer.None;
    TickTimer bulletFireDelay = TickTimer.None;

    //? network object nao tao ra tia raycast
    NetworkPlayer networkPlayer;
    NetworkObject networkObject;
    //[SerializeField] HPHandler hPHandler;

    //! testing
    [SerializeField] LocalCameraHandler localCameraHandler;
    float fireRateCurrent = 2f;
    [SerializeField] byte weaponDamageCurr = 1;
    Vector3 spawnPointRaycastCam = Vector3.zero;

    [Networked]
    public int killCount { get; set; }

    bool isFired = false;
    bool isFiredPressed = false;
    [SerializeField] float coolTimeWeapon = 0.5f;

    //others
    Spawner spawner;

    CharacterInputHandler characterInputHandler;
    HPHandler hPHandler;

    const string HEAD = "Head";
    const string ARML = "Shoulder_L";
    const string ARMR = "Shoulder_R";

    //[Header("Recoil")]
    //[SerializeField] float currentRecoilX = -2;
    //[SerializeField] float currentRecoilY = 2;
    //[SerializeField] float currentRecoilZ = 0.35f;
    //[SerializeField] float currentReturnSpeed = 2;
    //[SerializeField] float currentSnappiness = 6;
    [SerializeField] private RecoilGunSettings recoil = new();
    private void Awake()
    {
        characterInputHandler = GetComponent<CharacterInputHandler>();
        hPHandler = GetComponent<HPHandler>();
        networkPlayer = GetComponent<NetworkPlayer>();
        networkObject = GetComponent<NetworkObject>();
        localCameraHandler = FindFirstObjectByType<LocalCameraHandler>();

        //weaponSwitcher = GetComponent<WeaponSwitcher>();
        spawner = FindObjectOfType<Spawner>();
    }

    public override void Spawned()
    {
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);


    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Ready") return;
        if (HasStateAuthority == false) return;

        // nhan mouse 0 fire bullet
        //if(Input.GetKeyDown(KeyCode.Mouse0)) isFired = true;
        ////isFired = characterInputHandler.IsFired;
    }

    [SerializeField] private bool isSingleMode = false;
    public void SetFireInput(bool isFire)
    {
        isFired = isFire;
    }

    public override void FixedUpdateNetwork()
    {
        if (SceneManager.GetActiveScene().name == "MainLobby") return;
        if (Matchmaking.Instance != null)
        {
            if ((!Matchmaking.Instance.IsDone && Matchmaking.Instance.currentMode == Matchmaking.Mode.Solo)
            || (!MatchmakingTeam.Instance.IsDone && Matchmaking.Instance.currentMode == Matchmaking.Mode.Duo)) return;
        }
        if (Object.HasStateAuthority)
        {
            if (WeaponManager.instance.IsReadyToShoot() &&
                !hPHandler.Networked_IsDead && hPHandler.Networked_HP > 0)
            {
                Fire();

            }
        }

    }

    void Fire()
    {
        if (!isFiredPressed && isFired)
        {
            if(isSingleMode)
            {
                isFired = !isFired;
            }

            isFiredPressed = true;
            WeaponManager.instance.Shoot();
            StartCoroutine(FireCO(coolTimeWeapon));
        }

    }

    IEnumerator FireCO(float coolTime)
    {
        // chi tao ra hieu ung laser no o nong sung va bay toi muc tieu va cham
        localCameraHandler.RaycastHitPoint();
        localCameraHandler.SetRecoil(recoil);

        var hitPointVector3 = localCameraHandler.hitPoint_Network;

        if (hitPointVector3 != Vector3.zero)
        {
            if (!characterInputHandler.IsThirdCam)
            {
                FireBulletVFX(hitPointVector3, aimPoint_grandeRocket.position);
            }
            else FireBulletVFX(hitPointVector3, aimPoint_grandeRocket_3rd.position);
        }

        Fire(localCameraHandler.transform.forward, aimPoint);  // neu player thi aimpoint = vi tri 1st cam
        yield return new WaitForSeconds(coolTime);

        isFiredPressed = false;
    }

    public override void Render()
    {
        if (changeDetector == null) return;
        foreach (var change in changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(isFiring):
                    var boolReader = GetPropertyReader<bool>(nameof(isFiring));
                    var (previousBool, currentBool) = boolReader.Read(previousBuffer, currentBuffer);
                    OnFireChanged(previousBool, currentBool);
                    break;
            }
        }
    }

    //? fire bullet laser VFX => chi tao ra virtual o nong sung + bullet trails + impact
    void FireBulletVFX(Vector3 hitPoint, Vector3 spanwPoint)
    {
        Vector3 dir = dir = hitPoint - spanwPoint;

        if (bulletFireDelay.ExpiredOrNotRunning(Runner))
        {

            Runner.Spawn(bulletVFXPF, spanwPoint, Quaternion.LookRotation(dir), Object.InputAuthority,
            (runner, spawnBullet) =>
            {
                spawnBullet.GetComponent<BulletHandler>().FireBullet(Object.InputAuthority, networkObject, networkPlayer.nickName_Network.ToString());
            });
            bulletFireDelay = TickTimer.CreateFromSeconds(Runner, 0.15f); // sau 3 s se exp or notRunning
        }
    }

    //? FIRE raycast BULLET FROM CAMERA
    void Fire(Vector3 aimForwardVector, Transform aimPoint)
    {
        //? AI fire theo AI fireRate
        //if(networkPlayer.isBot && Time.time - lastTimeFired < aiFireRate) return;

        //? player fire rate theo lasTimeLimit
        if (Time.time - lastTimeFired < 0.15f) return;

        StartCoroutine(FireEffect());

        /* var spawnPointRaycastCam = localCameraHandler.raycastSpawnPointCam_Network; */

        //? neu la AI thi diem ban se la camera anrcho
        /* if(!networkPlayer.isBot) 
            spawnPointRaycastCam = localCameraHandler.raycastSpawnPointCam_Network;
        else spawnPointRaycastCam = aiCameraAnchor.position; */


        spawnPointRaycastCam = localCameraHandler.raycastSpawnPointCam_Network;

        if (Physics.Raycast(spawnPointRaycastCam, aimForwardVector, out var hit, 100, collisionLayers))
        {
            // neu hitInfo do this.gameObject ban ra thi return
            if (hit.transform.GetComponent<WeaponHandler>() == this) return;
            // neu hitInfo la dong doi thi khong tru mau
            //if (hit.transform.CompareTag("TeamMate")) return;
            if (IsTeammate(hit)) return;
            float hitDis = 100f;
            bool isHitOtherRemotePlayers = false;

            if (hit.distance > 0) hitDis = hit.distance;

            // check body part
            if (hit.collider.transform.TryGetComponent<CheckBodyParts>(out var part))
            {
                string bodyName = hit.collider.transform.name;
                Debug.Log($"_____bodyName = {bodyName}");
                if (bodyName == HEAD) weaponDamageCurr = hPHandler.Networked_HP;
                else if (bodyName == ARML || bodyName == ARMR) weaponDamageCurr = 1;

                if (Object.HasStateAuthority)
                {
                    /* hit.collider.GetComponent<HPHandler>().OnTakeDamage(networkPlayer.nickName_Network.ToString(), 1, this); */
                    part.hPHandler.OnTakeDamage(networkPlayer.nickName_Network.ToString(), weaponDamageCurr, this);
                }
            }
            else weaponDamageCurr = 1;

            // get damage ohters
            if (hit.transform.TryGetComponent<HPHandler>(out var health))
            {
                Debug.Log($"{Time.time} {transform.name} hit HitBox {hit.transform.root.name}");

                // ban trung dau get full hp
                string bodyName = hit.collider.transform.name;
                Debug.Log($"_____bodyName = {bodyName}");
                if (bodyName == HEAD) weaponDamageCurr = hPHandler.Networked_HP;
                else if (bodyName == ARML || bodyName == ARMR) weaponDamageCurr = 1;

                if (Object.HasStateAuthority)
                {
                    /* hit.collider.GetComponent<HPHandler>().OnTakeDamage(networkPlayer.nickName_Network.ToString(), 1, this); */
                    hit.collider.GetComponent<HitboxRoot>().GetComponent<HPHandler>().
                                OnTakeDamage(networkPlayer.nickName_Network.ToString(), weaponDamageCurr, this);
                }

                isHitOtherRemotePlayers = true;
            }
            else if (hit.collider != null)
            {
                Debug.Log($"{Time.time} {transform.name} hit PhysiX Collier {hit.transform.root.name}");
            }

            //? ve ra tia neu ban trung remotePlayers
            if (isHitOtherRemotePlayers)
                Debug.DrawRay(aimPoint.position, aimForwardVector * hitDis, Color.red, 1f);
            else
                Debug.DrawRay(aimPoint.position, aimForwardVector * hitDis, Color.green, 1f);

        }

        lastTimeFired = Time.time;

        // lam cho ai ban theo tan suat random khoang time
        //fireRateCurrent = Random.Range(0.1f, 1.5f);
    }

    private bool IsTeammate(RaycastHit hit)
    {
        string playerTeamID = GetComponent<PlayerRoomController>().TeamID.ToString();
        string targetTeamID = "";
        if (hit.transform.GetComponent<PlayerRoomController>() != null)
            targetTeamID = hit.transform.GetComponent<PlayerRoomController>()?.TeamID.ToString();
        return playerTeamID == targetTeamID && playerTeamID != "";
    }

    // fire particle on aimPoint
    IEnumerator FireEffect()
    {
        isFiring = true;

        //? show cho localPlayer thay hieu ung ban ra
        if (characterInputHandler.IsThirdCam)
            fireParticleSystemRemote.Play();
        else
            fireParticleSystemLocal.Play();

        if (audioSource)
        {
            audioSource.PlayOneShot(weaponSoundCurr, 0.5f);
        }

        yield return new WaitForSeconds(0.09f);
        isFiring = false;
    }

    void OnFireChanged(bool previous, bool current)
    {
        if (current && !previous)
            OnFireRemote();
    }

    void OnFireRemote()
    {
        if (!Object.HasStateAuthority)
        {
            fireParticleSystemRemote.Play();

            if (audioSource)
            {
                audioSource.PlayOneShot(weaponSoundCurr, 0.5f);
            }
        }


    }

    public void SetConfig(GunItemConfig config)
    {
        weaponSoundCurr = config.shootingSound;
        weaponDamageCurr = config.damagePerHit;
        coolTimeWeapon = config.cooldownTime;
        isSingleMode = config.isSingleMode;
        recoil = config.recoil;
    }

    public void Initialize()
    {
        WeaponManager.instance.weaponHandler = this;
    }
}