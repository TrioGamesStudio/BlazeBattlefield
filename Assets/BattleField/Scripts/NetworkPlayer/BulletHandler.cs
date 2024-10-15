using System.Collections.Generic;
using Fusion;
using UnityEngine;

//todo game object = bullet pf (co chua san vfx)
public class BulletHandler : NetworkBehaviour
{
    [Header("Prefabs")]
    [SerializeField ] ParticleSystem laserParticleBulletPF; // vfc on this.object

    //? fired by PLAYER INFO - WHO FIRED
    //timing
    TickTimer maxLiveDurationTickTimer = TickTimer.None; // thoi gian ton tai

    PlayerRef fireByPlayerRef;
    string fireByPlayerName;
    NetworkObject fireByNetworkObject;
    NetworkObject networkObject;

    
    [Header("Collision Detection")]
    [SerializeField] Transform checkForImpactPoint;
    Collider[] hits = new Collider[10];
    [SerializeField] LayerMask collisionLayerMask;

    public void FireBullet(PlayerRef fireByPlayerPref, NetworkObject fireByNetworkObject, string fireByPlayerName) {
        this.fireByPlayerRef = fireByPlayerPref;
        this.fireByPlayerName = fireByPlayerName;
        this.fireByNetworkObject = fireByNetworkObject;

        networkObject = GetComponent<NetworkObject>();

        maxLiveDurationTickTimer = TickTimer.CreateFromSeconds(Runner, 0.2f);
    }

    public override void FixedUpdateNetwork()
    {
        if(Object.HasStateAuthority) {
            if(maxLiveDurationTickTimer.Expired(Runner)) {
                Runner.Despawn(networkObject);
                return;
            }
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        
    }
}
