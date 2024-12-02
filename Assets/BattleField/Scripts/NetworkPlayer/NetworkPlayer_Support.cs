using Fusion;
using UnityEngine;
public class NetworkPlayer_Support : NetworkBehaviour
{
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
        
        var INetworkInitializes = GetComponentsInChildren<INetworkInitialize>();
        foreach(var item in INetworkInitializes)
        {
            item.Initialize();
        }

        if (Runner.IsSharedModeMasterClient)
        {
            RandomGroupManager.RaiseStartSpawnEvent();
        }
    }
}
public interface INetworkInitialize
{
    void Initialize();
}