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
        if (bulletFireDelay.ExpiredOrNotRunning(Runner))
        {

            Runner.Spawn(bulletVFXPF, aimPoint_grandeRocket_3rd.transform.position, Quaternion.LookRotation(dir), Object.InputAuthority,
            (runner, spawnBullet) =>
            {
                spawnBullet.GetComponent<BulletHandler>().FireBullet(Object.InputAuthority, networkObject, "bot");
            });
           
            bulletFireDelay = TickTimer.CreateFromSeconds(Runner, 0.15f); // sau 3s se exp or notRunning
        }

        //if (bulletFireDelay.ExpiredOrNotRunning(Runner))
        //{
        //    Runner.Spawn(bulletVFXPF, aimPoint_grandeRocket_3rd.transform.position, Quaternion.LookRotation(dir));
        //    bulletFireDelay = TickTimer.CreateFromSeconds(Runner, 0.15f); // sau 3 s se exp or notRunning
        //}
        FireRaycast(dir, aimPoint_grandeRocket_3rd);
    }


    void FireRaycast(Vector3 aimForwardVector, Transform aimPoint)
    {
        //spawnPointRaycastCam = localCameraHandler.raycastSpawnPointCam_Network;

        if (Physics.Raycast(aimPoint.position, aimForwardVector, out var hit, 100, opponentLayer))
        {
            Debug.Log(",,, Hit " + hit.transform.name);
           //byte localWeaponDamageCurr = 0;
            if (hit.transform.GetComponent<ActiveWeaponAI>() == this) return;

            float hitDis = 100f;
            bool isHitOtherRemotePlayers = false;

            if (hit.distance > 0) hitDis = hit.distance;

            // check body part
            if (hit.collider.transform.TryGetComponent<CheckBodyParts>(out var part))
            {
                string bodyName = hit.collider.transform.name;
                Debug.Log($"_____bodyName = {bodyName}");
                //    if (bodyName == HEAD) localWeaponDamageCurr = hPHandler.Networked_HP;
                //    else if (bodyName == ARML || bodyName == ARMR) localWeaponDamageCurr = this.weaponDamageCurr;

                //    if (Object.HasStateAuthority)
                //    {
                //        /* hit.collider.GetComponent<HPHandler>().OnTakeDamage(networkPlayer.nickName_Network.ToString(), 1, this); */
                //        isHit = true;
                //        part.hPHandler.OnTakeDamage(networkPlayer.nickName_Network.ToString(), localWeaponDamageCurr, this);
                //    }
                //    PlayerStats.Instance.AddDamageDealt(localWeaponDamageCurr);
                //}
                //else localWeaponDamageCurr = this.weaponDamageCurr;

                // get damage ohters
                if (hit.transform.TryGetComponent<HPHandler>(out var health))
                {
                    //Debug.Log($"{Time.time} {transform.name} hit HitBox {hit.transform.root.name}");

                    //// ban trung dau get full hp
                    //string bodyName = hit.collider.transform.name;
                    //Debug.Log($"_____bodyName = {bodyName}");
                    //if (bodyName == HEAD) localWeaponDamageCurr = hPHandler.Networked_HP;
                    //else if (bodyName == ARML || bodyName == ARMR) localWeaponDamageCurr = this.weaponDamageCurr;

                    //if (Object.HasStateAuthority)
                    //{
                    //    isHit = true;
                    //    Debug.LogWarning($"Damgage !!!!!{localWeaponDamageCurr} {weaponDamageCurr}");
                    //    /* hit.collider.GetComponent<HPHandler>().OnTakeDamage(networkPlayer.nickName_Network.ToString(), 1, this); */
                    //    hit.collider.GetComponent<HitboxRoot>().GetComponent<HPHandler>().
                    //                OnTakeDamage(networkPlayer.nickName_Network.ToString(), localWeaponDamageCurr, this);
                    //}
                    //PlayerStats.Instance.AddDamageDealt(localWeaponDamageCurr);
                    isHitOtherRemotePlayers = true;
                }
                //else if (hit.collider != null)
                //{
                //    Debug.Log($"{Time.time} {transform.name} hit PhysiX Collier {hit.transform.root.name}");
                //}
                //Debug.LogWarning($"Damgage !!!!!{localWeaponDamageCurr} {weaponDamageCurr}");
                ////? ve ra tia neu ban trung remotePlayers
                if (isHitOtherRemotePlayers)
                    Debug.DrawRay(aimPoint.position, aimForwardVector * hitDis, Color.red, 1f);
                else
                    Debug.DrawRay(aimPoint.position, aimForwardVector * hitDis, Color.green, 1f);
            }

            //lastTimeFired = Time.time;
        }

    }
}
