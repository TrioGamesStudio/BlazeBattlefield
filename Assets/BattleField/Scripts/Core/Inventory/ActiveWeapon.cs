using Fusion;
using NaughtyAttributes;
using UnityEngine;

public class ActiveWeapon : NetworkBehaviour
{
    public Transform[] weaponHoldersLocal;
    public Transform[] weaponHoldersRemote;

    public WeaponSlotHandler[] WeaponSlotHandlers;
    public GameObject currentWeaponLocal;
    public GameObject currentWeaponRemote;

    private int currentIndex = 0;
    public bool isHoslter = false;

    private void Awake()
    {
        WeaponSlotHandlers = WeaponManager.instance.WeaponSlotHandlers;
        WeaponManager.instance.activeWeapon = this;
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
    public void Drop()
    {
        HideAllSlots();
        Destroy(currentWeaponLocal.gameObject);
        Destroy(currentWeaponRemote.gameObject);
        WeaponSlotHandlers[currentIndex].DeleteAndSpawnWorld();
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

    public WeaponSlotHandler GetCurrentWeapon()
    {
        return WeaponSlotHandlers[currentIndex];
    }
}
