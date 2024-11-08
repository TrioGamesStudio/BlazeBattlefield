using Fusion;
using UnityEngine;
public class NetworkPlayer_Support : NetworkBehaviour
{
    [SerializeField] ActiveWeapon activeWeapon;
    [SerializeField] private Animator animator;

    public override void Spawned()
    {
        base.Spawned();
        WeaponManager.instance.playerAnimator = animator;
        WeaponManager.instance.ShowWeapon(false);
        ItemDatabase.instance.PlayerObject = transform;
        activeWeapon.Init();
    }
}
