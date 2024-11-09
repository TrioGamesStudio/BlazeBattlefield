using Fusion;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using static ActiveWeapon;
public partial class ActiveWeapon : NetworkBehaviour
{

    public Transform[] weaponHoldersLocal;
    public Transform[] weaponHoldersRemote;
    private WeaponHolder[] weaponHolders;
    [Networked, Capacity(4)] NetworkDictionary<byte, bool> activeWeaponState => default;
    public void Init()
    {
        //WeaponSlotHandlers = WeaponManager.instance.WeaponSlotHandlers;
        if (WeaponManager.instance == null) return;
        WeaponManager.instance.activeWeapon = this;
        weaponHolders = new WeaponHolder[4];
        for (int i = 0; i < weaponHolders.Length; i++)
        {
            weaponHolders[i] = new();
            var weaponHolder = weaponHolders[i];
            var localTransform = weaponHoldersLocal[i];
            var remoteTransform = weaponHoldersRemote[i];
            weaponHolder.SetWeaponSlotHandler(WeaponManager.instance.WeaponSlotHandlers[i]);
            weaponHolder.index = i;
            weaponHolder.activeWeapon = this;
        }
    }

    private NetworkObject SpawnItem(GameObject prefab, bool isLocal, int index, string _tag)
    {
        var position = isLocal ? weaponHoldersLocal[index].position : weaponHoldersRemote[index].position;
        var networkObject = Runner.Spawn(prefab, position, Quaternion.Euler(0, 0, 0), null, (runner, obj) =>
        {
            obj.GetComponent<BoundItem>().allowAddToCollider = false;
            obj.GetComponent<TagObjectHandler>().ObjectTag = _tag;
        });
        Debug.Log("Start set parent", gameObject);
        RPC_SetParentWeapon(networkObject, isLocal, index);
        return networkObject;
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void ShowWeapon_RPC(int activeIndex)
    {
        //SetActiveList(false, weaponHoldersLocal);
        //SetActiveList(false, weaponHoldersRemote);
        weaponHoldersLocal[activeIndex].gameObject.SetActive(true);
        weaponHoldersRemote[activeIndex].gameObject.SetActive(true);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void HideWeapon_RPC()
    {
        SetActiveList(false, weaponHoldersLocal);
        SetActiveList(false, weaponHoldersRemote);
    }

    private void SetActiveList(bool isActive, Transform[] list)
    {
        foreach (var item in list)
        {
            item.gameObject.SetActive(isActive);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetParentWeapon(NetworkObject weapon, bool isLocal, int index)
    {
        weapon.transform.SetParent(isLocal ? weaponHoldersLocal[index] : weaponHoldersRemote[index], false);
        weapon.GetComponent<NetworkTransform>().Teleport(isLocal ? weaponHoldersLocal[index].position : weaponHoldersRemote[index].position);
        //weapon.transform.SetParent(parent.transform);
        //Debug.Log($"Weapon name {weapon.name}");
    }

}
