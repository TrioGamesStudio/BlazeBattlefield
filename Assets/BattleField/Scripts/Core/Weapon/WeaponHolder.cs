using Fusion;
using UnityEngine;
public partial class ActiveWeapon
{
    public class WeaponHolder
    {
        public NetworkObject currentWeaponLocal;
        public NetworkObject currentWeaponRemote;

        public WeaponSlotHandler weaponSlotHandler;

        public ActiveWeapon activeWeapon;

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
            activeWeapon.ShowWeapon_RPC(index);
        }

        public void Hide()
        {
            activeWeapon.HideWeapon_RPC();
        }

        private void Equip()
        {
            Quaternion quaternion = Quaternion.Euler(0, 0, 0);

            currentWeaponLocal = activeWeapon.SpawnItem(weaponSlotHandler.Prefab, true, index, "IgnoreLayerChange");
            currentWeaponRemote = activeWeapon.SpawnItem(weaponSlotHandler.Prefab, false, index, "Untagged");

            activeWeapon.SetRenderForLocalAndRomoteBody();
            
        }

        private void Drop()
        {
            activeWeapon.Runner.Despawn(currentWeaponLocal);
            activeWeapon.Runner.Despawn(currentWeaponRemote);
            currentWeaponLocal = null;
            currentWeaponRemote = null;
        }
    }

}
