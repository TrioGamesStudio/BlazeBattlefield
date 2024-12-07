using Fusion;
using UnityEngine;
public class NetworkPlayer_Support : NetworkBehaviour
{
    [SerializeField] private Animator animator;
    public void Init()
    {
        // this method call with object has state authority
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

        if (StartGameHandler.instance)
        {
            GetComponent<CharacterMovementHandler>().Respawn();
        }
    }
}
public interface INetworkInitialize
{
    void Initialize();
}