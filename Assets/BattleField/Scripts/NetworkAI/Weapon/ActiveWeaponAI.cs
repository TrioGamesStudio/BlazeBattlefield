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
        //NetworkObject networkObject = Runner.Spawn(prefab, position, Quaternion.identity);
        //NetworkObject networkObject = Runner.Spawn(prefab, position);
        GameObject weapon = Instantiate(prefab, position, Quaternion.identity);
        RPC_SetParentWeapon(weapon.GetComponent<NetworkObject>(), index);
        return weapon.GetComponent<NetworkObject>();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetParentWeapon(NetworkObject weapon, int index)
    {
        //weapon.transform.SetParent(weaponHoldersRemote[index], false);
        //weapon.GetComponent<NetworkTransform>().Teleport(isLocal ? weaponHoldersLocal[index].position : weaponHoldersRemote[index].position);
        weapon.transform.SetParent(weaponHoldersRemote[index].transform);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        //Debug.Log($"Weapon name {weapon.name}"); */
    }

    //? fire bullet laser VFX => chi tao ra virtual o nong sung + bullet trails + impact
    public void Fire(Transform target)
    {
        Vector3 dir = target.position - aimPoint_grandeRocket_3rd.position;
        //if (bulletFireDelay.ExpiredOrNotRunning(Runner))
        //{

        //    Runner.Spawn(bulletVFXPF, aimPoint_grandeRocket_3rd.transform.position, Quaternion.LookRotation(dir), Object.InputAuthority,
        //    (runner, spawnBullet) =>
        //    {
        //        spawnBullet.GetComponent<BulletHandler>().FireBullet(Object.InputAuthority, networkObject, "bot");
        //    });
        //    bulletFireDelay = TickTimer.CreateFromSeconds(Runner, 0.15f); // sau 3 s se exp or notRunning
        //}

        if (bulletFireDelay.ExpiredOrNotRunning(Runner))
        {
            Runner.Spawn(bulletVFXPF, aimPoint_grandeRocket_3rd.transform.position, Quaternion.LookRotation(dir));
            bulletFireDelay = TickTimer.CreateFromSeconds(Runner, 0.15f); // sau 3 s se exp or notRunning
        }
       
    }
}
