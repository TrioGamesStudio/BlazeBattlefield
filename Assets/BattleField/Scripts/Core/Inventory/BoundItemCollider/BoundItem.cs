using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundItem : NetworkBehaviour
{
    [SerializeField] private bool isInBoundCollider = false;
    [SerializeField] private float height = 1.2f;
    public bool IsInBoundCollider { get => isInBoundCollider; set => isInBoundCollider = value; }
    public BoundItemsCollider BoundItemsCollider;
    [Networked] public bool allowAddToCollider { get; set; }

    public void SetupFromStateAuthority()
    {
        ColliderCreator.instance.Add(this);
    }

    public override void Spawned()
    {
        base.Spawned();
        Invoke(nameof(SetupFromStateAuthority), .5f);
    }


    [EditorButton]
    private void AddToColliderCreator()
    {
        if (allowAddToCollider)
        {
            ColliderCreator.instance.Add(this);
        }
    }


    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);

        if (BoundItemsCollider != null)
        {
            BoundItemsCollider.RemoveItemFromBoundss(this);
        }
    }



    public bool CanAddToBound()
    {
        return isInBoundCollider == false && allowAddToCollider && BoundItemsCollider == null;
    }
}
