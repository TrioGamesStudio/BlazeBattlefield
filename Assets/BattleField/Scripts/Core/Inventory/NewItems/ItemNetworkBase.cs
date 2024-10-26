using Fusion;
using System;

public abstract class ItemNetworkBase : NetworkBehaviour, RunTimeItem
{
    
    [Networked] public int quantity { get; set; }
    public Action<RunTimeItem> OnRemoveItemUI { get; set; }
    public bool isDisplayedUI { get; set; }

    public string displayName;
   
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        if (isDisplayedUI)
        {
            OnRemoveItemUI?.Invoke(this);
        }
    }
    public string GetItemName()
    {
        return displayName;
    }

    public int GetQuantity()
    {
        return quantity;
    }

    public abstract void Collect();

    public string GetUniqueID()
    {
        return Object.NetworkTypeId.ToString();
    }
    public void DestroyItem()
    {
        DestroyRPC();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void DestroyRPC()
    {
        if (HasStateAuthority)
        {
            Runner.Despawn(Object);
        }
    }
 
}