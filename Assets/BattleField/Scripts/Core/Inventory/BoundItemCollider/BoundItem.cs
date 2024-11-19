using Fusion;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundItem : NetworkBehaviour
{
    [SerializeField] private bool isInBoundCollider = false;
    [SerializeField] private float height = 1.2f;
    public bool IsInBoundCollider { get => isInBoundCollider; set => isInBoundCollider =value; }
    public BoundItemsCollider BoundItemsCollider;
    [SerializeField] private BoxCollider _collider;
    [Networked] public bool allowAddToCollider { get; set; }
    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
    }

    public override void Spawned()
    {
        base.Spawned();
    }

    public void Setup()
    {
        if(allowAddToCollider)
        {
            SetItemNearGround();
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
    [EditorButton]
    private void SetItemNearGround()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, float.MaxValue))
        {
            if (hit.collider.gameObject.CompareTag("Ground"))
            {
                //Debug.Log("Set item position near to ground", gameObject);
                Vector3 spawnPosition = hit.point + new Vector3(0, _collider.size.y / 2, 0);
                transform.position = spawnPosition;
                //Debug.LogWarning($"Method: {spawnPosition}");
                //Debug.Log("Set spawn Position", gameObject);
            }
        }
    }

    private void OnDrawGizmos()
    {

        if (Physics.Raycast(transform.position, Vector3.down, out var hit, float.MaxValue))
        {
            Gizmos.DrawLine(transform.position, hit.point);
            Gizmos.color = Color.red;
            //float y = _collider.size.y * transform.localScale.y;
            //Gizmos.DrawCube(hit.point + new Vector3(0, y, 0), _collider.size * transform.localScale.x);
            var spawnPos = hit.point + new Vector3(0, _collider.size.y / 2, 0);
            Gizmos.DrawWireCube(spawnPos, _collider.size);
            //Debug.Log($"Gizmo: {spawnPos}");
        }
    }

    public bool CanAddToBound()
    {
        return isInBoundCollider == false && allowAddToCollider && BoundItemsCollider == null;
    }

    public void ExitBound()
    {
        BoundItemsCollider = null;
        allowAddToCollider = true;
        isInBoundCollider = false;
    }
}
