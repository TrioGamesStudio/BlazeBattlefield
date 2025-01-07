using DG.Tweening;
using Fusion;
using System.Collections;
using UnityEngine;
public class NetworkPlayer_Support : NetworkBehaviour
{
    [SerializeField] private Animator animator;
    public void Awake()
    {
        if (HasStateAuthority)
        {
            CameraEffectControl.instance.EyeBlinkEffect.CloseEyeImmediately();
        }
    }
    public void Init()
    {
        // this method call with object has state authority
        if (StartGameHandler.instance)
        {
            GetComponent<CharacterMovementHandler>().Respawn();
        }

        if (WeaponManager.instance != null)
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

        
        CameraEffectControl.instance.EyeBlinkEffect.OpenEye();
    }

    //public void PlayerLeft(PlayerRef player)
    //{
    //    if (GetComponent<HPHandler>().Networked_HP <= 0) return;
    //    StartCoroutine(Test());
    //}
    //private IEnumerator Test()
    //{
    //    yield return new WaitForSeconds(.5f);

    //    AlivePlayerControl.UpdateAliveCount(1);
    //}
}
public interface INetworkInitialize
{
    void Initialize();
}