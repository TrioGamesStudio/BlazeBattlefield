using Fusion;
using UnityEngine;
public class NetworkPlayer_Support : NetworkBehaviour
{
    [SerializeField] ActiveWeapon activeWeapon;
    [SerializeField] private Animator animator;
   
    public void Init()
    {
        if(WeaponManager.instance != null)
        {
            WeaponManager.instance.playerAnimator = animator;
            WeaponManager.instance.ShowWeapon(false);
        }
        if(ItemDatabase.instance != null)
        {
            ItemDatabase.instance.PlayerObject = transform;
        }
        activeWeapon.Init();
    }
}
