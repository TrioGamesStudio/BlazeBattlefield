using Fusion;
using UnityEngine;
public class TagObjectHandler : NetworkBehaviour
{

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void SetTag_RPC(string newTag)
    {
        tag = newTag;
    }
}
