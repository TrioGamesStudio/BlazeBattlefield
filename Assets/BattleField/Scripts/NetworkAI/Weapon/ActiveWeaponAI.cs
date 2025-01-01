using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ActiveWeaponAI : NetworkBehaviour
{
    [SerializeField] BulletHandler bulletVFXPF;
    [SerializeField] Transform aimPoint_grandeRocket_3rd; // VI TRI TREN NONG SUNG trong 3rdPersonCam
    public Transform[] weaponHoldersRemote;
    public NetworkObject currentWeaponRemote;
    //public ActiveWeapon activeWeapon;
    public GameObject weapon;
    Animator anim;

    //? network object nao tao ra tia raycast
    NetworkPlayer networkPlayer;
    NetworkObject networkObject;

    TickTimer bulletFireDelay = TickTimer.None;
    [SerializeField] private LayerMask opponentLayer;
    [SerializeField] byte weaponDamageCurr = 1;
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

    public void Equip(GameObject weaponPrefab)
    {
        anim.SetBool("isEquiped", true);
        weapon = weaponPrefab;
        Quaternion quaternion = Quaternion.Euler(0, 0, 0);
        currentWeaponRemote = SpawnItem(weapon, 0);
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

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetParentWeapon(NetworkObject weapon, int index)
    {
        weapon.transform.SetParent(weaponHoldersRemote[index].transform);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
    }

    // fire bullet laser VFX => chi tao ra virtual o nong sung + bullet trails + impact
    public void Fire(Transform target)
    {
        Vector3 dir = target.position - aimPoint_grandeRocket_3rd.position;
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
}
