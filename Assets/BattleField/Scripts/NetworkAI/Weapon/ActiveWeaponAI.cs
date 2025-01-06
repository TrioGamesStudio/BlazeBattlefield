using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ActiveWeaponAI : NetworkBehaviour
{
    [Networked]
    private bool HasGunNetworked { get; set; }
    public NetworkObject currentWeaponRemote;
    public GameObject weapon;

    [SerializeField] BulletHandler bulletVFXPF;
    [SerializeField] Transform aimPoint_grandeRocket_3rd; // VI TRI TREN NONG SUNG trong 3rdPersonCam
    public Transform[] weaponHoldersRemote;
   
    //public ActiveWeapon activeWeapon;
    
    Animator anim;

    //? network object nao tao ra tia raycast
    NetworkPlayer networkPlayer;
    NetworkObject networkObject;

    TickTimer bulletFireDelay = TickTimer.None;
    [SerializeField] private LayerMask opponentLayer;
    [SerializeField] byte weaponDamageCurr = 1;

    [Header("Sound")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip weaponSoundCurr;

    [Networked] // bien updated through the server on all the clients
    public bool isFiring { get; set; }
    ChangeDetector changeDetector;
    public bool HasGun;

    public override void Spawned()
    {
        currentWeaponRemote = null;
        HasGun = false;
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    private void Awake()
    {
        //networkPlayer = GetComponent<NetworkPlayer>();
        networkObject = GetComponent<NetworkObject>();
    }

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        //Equip();
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

    public override void FixedUpdateNetwork()
    {
        //HasGun = currentWeaponRemote != null;
        //if (!HasGun)
        //{
        //    anim.SetBool("isEquiped", false);
        //}
    }

    public void Equip(GameObject weaponPrefab)
    {
        //if (weaponPrefab == null) return;    
        weapon = weaponPrefab;
        currentWeaponRemote = SpawnItem(weapon, 0);
        if (currentWeaponRemote)
        {
            anim.SetBool("isEquiped", true);
            HasGun = true;
        }
    }

    //Spawn weapon
    private NetworkObject SpawnItem(GameObject prefab, int index)
    {
        var position = weaponHoldersRemote[index].position;

        //GameObject weapon = Instantiate(prefab, position, Quaternion.identity);
        //RPC_SetParentWeapon(weapon.GetComponent<NetworkObject>(), index);
        //return weapon.GetComponent<NetworkObject>();

        NetworkObject networkObject = Runner.Spawn(prefab, position, Quaternion.identity);
        RPC_SetParentWeapon(networkObject, index);
        return networkObject;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SetParentWeapon(NetworkObject weapon, int index)
    {
        weapon.gameObject.layer = 0;
        weapon.transform.SetParent(weaponHoldersRemote[index].transform);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        weapon.GetComponent<Outline>().enabled = false;
    }

    // fire bullet laser VFX => chi tao ra virtual o nong sung + bullet trails + impact
    public void Fire(Transform target)
    {
        Vector3 offsetFiringPos = new(0, 1, 0);
        Vector3 dir = (target.position + offsetFiringPos) - aimPoint_grandeRocket_3rd.position;
        FireRaycast(dir, aimPoint_grandeRocket_3rd);

        //if (bulletFireDelay.ExpiredOrNotRunning(Runner))
        {

            Runner.Spawn(bulletVFXPF, aimPoint_grandeRocket_3rd.transform.position, Quaternion.LookRotation(dir), Object.InputAuthority,
            (runner, spawnBullet) =>
            {
                spawnBullet.GetComponent<BulletHandler>().FireBullet(Object.InputAuthority, networkObject, "bot");
            });
           
            //bulletFireDelay = TickTimer.CreateFromSeconds(Runner, 0.15f); // sau 3s se exp or notRunning
        }      
    }


    void FireRaycast(Vector3 aimForwardVector, Transform aimPoint)
    {
        StartCoroutine(FireEffect());
        //Debug.Log("--- Fire raycast ne");
        if (Physics.Raycast(aimPoint.position, aimForwardVector, out var hit, 20, opponentLayer))
        {
            Debug.Log("--- Hit " + hit.transform.name);
            float hitDis = 20f;
            if (hit.transform.name != null)
                Debug.DrawRay(aimPoint.position, aimForwardVector * hitDis, Color.green, 1f);
            else
                Debug.DrawRay(aimPoint.position, aimForwardVector * hitDis, Color.red, 1f);
            //byte localWeaponDamageCurr = 0;
            if (hit.transform.GetComponent<ActiveWeaponAI>() == this) return;

            //// check body part
            if (hit.collider.transform.TryGetComponent<CheckBodyParts>(out var part))
            {
                //if (Object.HasStateAuthority)
                {
                    //isHit = true;
                    if (part != null)
                    {
                        HPHandler hp = part.hPHandler;
                        if (hp != null)
                        {
                            hp.OnTakeDamage("bot", weaponDamageCurr, null);
                        }
                    }                 
                }
                //PlayerStats.Instance.AddDamageDealt(localWeaponDamageCurr);
            }
            //else localWeaponDamageCurr = this.weaponDamageCurr;

            // get damage ohters
            if (hit.transform.TryGetComponent<HPHandler>(out var health))
            {
                if (Object.HasStateAuthority)
                {
                    hit.collider.GetComponent<HitboxRoot>().GetComponent<HPHandler>().
                                OnTakeDamage("bot", weaponDamageCurr, null);
                }
                //PlayerStats.Instance.AddDamageDealt(localWeaponDamageCurr);
            }
        }

    }

    // fire particle on aimPoint
    IEnumerator FireEffect()
    {
        isFiring = true;

        ////? show cho localPlayer thay hieu ung ban ra
        //if (characterInputHandler.IsThirdCam)
        //    fireParticleSystemRemote.Play();
        //else
        //    fireParticleSystemLocal.Play();

        audioSource.PlayOneShot(weaponSoundCurr, 0.5f);

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
            //fireParticleSystemRemote.Play();
            if (audioSource)
            {
                audioSource.PlayOneShot(weaponSoundCurr, 0.5f);
            }
        }
    }
}
