using Fusion;
using NaughtyAttributes;
using System;
using UnityEngine;
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

        public GameObject currentWeaponLocal;
        public GameObject currentWeaponRemote;

        public WeaponSlotHandler weaponSlotHandler;
        private IWeaponSlotAction iWeaponAction;
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
            currentWeaponLocal = Instantiate(weaponSlotHandler.Prefab, localHolderTransform.position, quaternion, localHolderTransform);
            currentWeaponLocal.tag = "IgnoreLayerChange";
            currentWeaponRemote = Instantiate(weaponSlotHandler.Prefab, remoteHolderTransform.position, quaternion, remoteHolderTransform);
            currentWeaponRemote.tag = "Untagged";
        }

        public void Drop()
        {
            Destroy(currentWeaponLocal.gameObject);
            Destroy(currentWeaponRemote.gameObject);
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

    //public WeaponSlotHandler[] WeaponSlotHandlers;
    public GameObject currentWeaponLocal;
    public GameObject currentWeaponRemote;

    private int currentIndex = 0;
    public bool isHoslter = false;
    private WeaponHolder[] weaponHolders;
    private void Awake()
    {
        //WeaponSlotHandlers = WeaponManager.instance.WeaponSlotHandlers;
        WeaponManager.instance.activeWeapon = this;
        weaponHolders = new WeaponHolder[4];
        for (int i = 0; i < weaponHolders.Length; i++)
        {
            var weaponHolder = new WeaponHolder();
            var localTransform = weaponHoldersLocal[i];
            var remoteTransform = weaponHoldersRemote[i];
            weaponHolder.SetSpawnTransform(localTransform, remoteTransform);
            weaponHolder.SetWeaponSlotHandler(WeaponManager.instance.WeaponSlotHandlers[i]);
        }
    }

    public override void Spawned()
    {
        base.Spawned();

    }

    public void Equip(WeaponSlotHandler WeaponSlotHandler)
    {
        HideAllSlots();

        // Instantiate
        currentIndex = (int)WeaponSlotHandler.Config.slotWeaponIndex;
        Show(currentIndex, true);
        SpawnWeapon(WeaponSlotHandler.Prefab, currentIndex);

        weaponHolders[currentIndex].Show();
    }

    public void Active(WeaponSlotHandler WeaponSlotHandler)
    {
        Show(this.currentIndex, false);
        int newIndex = (int)WeaponSlotHandler.Config.slotWeaponIndex;
        Show(newIndex, true);
    }

    private void SpawnWeapon(GameObject prefab, int index)
    {
        Quaternion quaternion = Quaternion.Euler(0, 0, 0);
        currentWeaponLocal = Instantiate(prefab, weaponHoldersLocal[index].position, quaternion, weaponHoldersLocal[index]);
        currentWeaponLocal.tag = "IgnoreLayerChange";
        currentWeaponRemote = Instantiate(prefab, weaponHoldersRemote[index].position, quaternion, weaponHoldersRemote[index]);
        currentWeaponRemote.tag = "Untagged";
    }

    private void Show(int index, bool isShow)
    {
        weaponHoldersLocal[index].gameObject.SetActive(isShow);
        weaponHoldersRemote[index].gameObject.SetActive(isShow);

    }


    public void Swap()
    {

    }

    public void Shoot()
    {

    }
    [EditorButton]
    public void Drop(WeaponSlotHandler weaponSlotHandler)
    {
        HideAllSlots();
        Destroy(currentWeaponLocal.gameObject);
        Destroy(currentWeaponRemote.gameObject);
        //WeaponSlotHandlers[currentIndex].DeleteAndSpawnWorld();
        currentWeaponLocal = null;
        //NOTE: viet ham drop gun
    }

    private void HideAllSlots()
    {
        foreach (var item in weaponHoldersLocal)
        {
            item.gameObject.SetActive(false);
        }
        foreach (var item in weaponHoldersRemote)
        {
            item.gameObject.SetActive(false);
        }
    }


}
