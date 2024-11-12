using Fusion;
using UnityEngine;
public class TagObjectHandler : NetworkBehaviour
{

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void SetTag_RPC(string newTag, bool isLocal)
    {
        tag = newTag;
        if (isLocal)
        {
            //Utils.SetRenderLayerInChildren(transform, LayerMask.NameToLayer("Default"));
            foreach (var trans in transform.GetComponentsInChildren<Transform>(true))
            {
                //trans.gameObject.layer = LayerMask.NameToLayer("Default");
            }
        }
        else
        {
            //Utils.SetRenderLayerInChildren(transform, LayerMask.NameToLayer("LocalPlayerModel"));
        }
    }
}
