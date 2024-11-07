using Fusion;
using NaughtyAttributes;
using System;
using UnityEngine;
using static ActiveWeapon;
public interface IWeaponSlotAction
{
    Action ShowWeaponAction { get; set; }
    Action HideWeaponAction { get; set; }
    Action EquipWeaponAction { get; set; }
    Action DropWeaponAction { get; set; }
}
public class ActiveWeapon : NetworkBehaviour
{
    public class WeaponHolder
    {
        public Transform localHolderTransform;
        public Transform remoteHolderTransform;

        public NetworkObject currentWeaponLocal;
        public NetworkObject currentWeaponRemote;

        public WeaponSlotHandler weaponSlotHandler;

        public ActiveWeapon activeWeapon;

        public NetworkRunner Runner;
        private IWeaponSlotAction iWeaponAction;
        public int index;

        public void SetWeaponSlotHandler(WeaponSlotHandler weaponSlotHandler)
        {
            this.weaponSlotHandler = weaponSlotHandler;
            iWeaponAction = weaponSlotHandler;
            Initialize();
        }

        private void Initialize()
        {
            iWeaponAction.ShowWeaponAction += Show;
            iWeaponAction.HideWeaponAction += Hide;
            iWeaponAction.EquipWeaponAction += Equip;
            iWeaponAction.DropWeaponAction += Drop;
        }

        public void Show()
        {
            ShowWeapon(true);
        }

        public void Hide()
        {
            ShowWeapon(false);
        }

        private void ShowWeapon(bool isShow)
        {
            currentWeaponLocal.gameObject.SetActive(isShow);
            currentWeaponRemote.gameObject.SetActive(isShow);
        }

        private void Equip()
        {
            Quaternion quaternion = Quaternion.Euler(0, 0, 0);
            //currentWeaponLocal = Instantiate(weaponSlotHandler.Prefab, localHolderTransform.position, quaternion, localHolderTransform);
            //currentWeaponLocal.tag = "IgnoreLayerChange";
            //currentWeaponRemote = Instantiate(weaponSlotHandler.Prefab, remoteHolderTransform.position, quaternion, remoteHolderTransform);
            //currentWeaponRemote.tag = "Untagged";

            currentWeaponLocal = SpawnItem("IgnoreLayerChange", localHolderTransform);
            currentWeaponRemote = SpawnItem("Untagged", remoteHolderTransform);
            activeWeapon.RPC_SetParentWeapon(currentWeaponLocal, true, index);
            activeWeapon.RPC_SetParentWeapon(currentWeaponRemote, false, index);
        }

        private NetworkObject SpawnItem(string tag, Transform spawnTransform)
        {
            var networkObject = Runner.Spawn(weaponSlotHandler.Prefab, spawnTransform.position, Quaternion.identity, null, (runner, obj) =>
            {
                obj.GetComponent<BoundItem>().allowAddToCollider = false;
                obj.GetComponent<TagObjectHandler>().ObjectTag = tag;

            });
            return networkObject;
        }


        public void Drop()
        {
            Runner.Despawn(currentWeaponLocal);
            Runner.Despawn(currentWeaponRemote);
            currentWeaponLocal = null;
            currentWeaponRemote = null;
        }

        public void SetSpawnTransform(Transform localTransform, Transform remoteTransform)
        {
            localHolderTransform = localTransform;
            remoteHolderTransform = remoteTransform;
        }

    }
    public Transform[] weaponHoldersLocal;
    public Transform[] weaponHoldersRemote;

    private WeaponHolder[] weaponHolders;
    public void Init()
    {
        //WeaponSlotHandlers = WeaponManager.instance.WeaponSlotHandlers;
        WeaponManager.instance.activeWeapon = this;
        weaponHolders = new WeaponHolder[4];
        for (int i = 0; i < weaponHolders.Length; i++)
        {
            weaponHolders[i] = new();
            var weaponHolder = weaponHolders[i];
            var localTransform = weaponHoldersLocal[i];
            var remoteTransform = weaponHoldersRemote[i];
            weaponHolder.SetSpawnTransform(localTransform, remoteTransform);
            weaponHolder.SetWeaponSlotHandler(WeaponManager.instance.WeaponSlotHandlers[i]);
            weaponHolder.Runner = Runner;
            weaponHolder.index = i;
            weaponHolder.activeWeapon = this;
        }
    }

    public override void Spawned()
    {
        base.Spawned();

    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetParentWeapon(NetworkObject weapon, bool isLocal, int index)
    {
        if (isLocal)
        {
            Debug.Log("Set local");
            weapon.transform.SetParent(weaponHoldersLocal[index]);
        }
        else
        {
            Debug.Log("Set remote");
            weapon.transform.SetParent(weaponHoldersRemote[index]);
        }
        //weapon.transform.SetParent(parent.transform);
        Debug.Log($"Weapon name {weapon.name}");
    }
    [EditorButton]
    public void TestRPC()
    {
        Debug.Log(weaponHolders[0].currentWeaponRemote);
        Debug.Log(weaponHolders[0].currentWeaponLocal);

    }

}
