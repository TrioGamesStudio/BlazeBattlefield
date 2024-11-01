using Fusion;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundItem : NetworkBehaviour
{
    public bool isInBound = false;
    public BoundItemsCollider BoundItemsCollider;
    [SerializeField] private BoxCollider _collider;
    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
    }

    public override void Spawned()
    {
        base.Spawned();
        ColliderCreator.instance.Add(this);
        SetItemNearGround();
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
}
