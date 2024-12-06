using Fusion;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class ItemGroundPositioner : NetworkBehaviour
{
    public static ItemGroundPositioner instance;
    public LayerMask GroundLayerMask;
    public float heightCheck = 50;
    private bool isProcess = false;

    private List<BoxCollider> colliderNeedToProcess = new();
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (colliderNeedToProcess.Count > 0 && isProcess == false)
        {
            isProcess = true;
            ProcessingItem(colliderNeedToProcess[0]);
        }
    }


    private RaycastHit[] raycastHits = new RaycastHit[10];
    public void SetItemNearGround(BoxCollider _collider)
    {
        colliderNeedToProcess.Add(_collider);
    }

    private void ProcessingItem(BoxCollider _collider)
    {
        if (_collider == null)
        {
            isProcess = false;
            return;
        }

        Vector3 rayOrigin = _collider.transform.position;
        Debug.Log("Start check position" + rayOrigin);
        for (int i = 0; i < 10; i++)
        {
            if (SpawnPosNotInCollider(rayOrigin, _collider.size / 2))
            {
                Debug.Log($"Grounded item:");
                break;
            }
            else
            {
                Debug.Log("You item in inside collider");
                rayOrigin += Vector3.up;
            }
        }
        Debug.Log("End check position" + rayOrigin);

        Ray ray = new Ray(rayOrigin, Vector3.down);
        ClearRaycastHits();
        int hits = Physics.RaycastNonAlloc(ray, raycastHits, 100, GroundLayerMask, QueryTriggerInteraction.Ignore);

        if (hits == 0)
        {
            Debug.LogWarning($"No ground found for {_collider.gameObject.name}");
            ResetProcess(_collider);
            return;
        }

        var groundHit = raycastHits
            .Take(hits)
            .Where(hit => hit.collider.CompareTag("Ground"))
            .OrderBy(hit => hit.distance)
            .FirstOrDefault();

        if (groundHit.collider != null)
        {
            //Vector3 spawnPosition = groundHit.point + Vector3.up * (_collider.size.y / 2);
            Vector3 spawnPosition = groundHit.point + Vector3.up * (_collider.size.y / 2);
            //_collider.transform.position = spawnPosition;
            if (_collider.TryGetComponent(out NetworkTransform networkTransform))
            {
                networkTransform.Teleport(spawnPosition);
            }
            Debug.Log($"Grounded item: {groundHit.collider.gameObject.name} at {spawnPosition}", _collider.gameObject);

        }
        else
        {
            Debug.LogWarning($"No suitable ground found for {_collider.gameObject.name}");
        }
        ResetProcess(_collider);
    }

    private void ClearRaycastHits()
    {
        for (int i = 0; i < raycastHits.Length; i++)
        {
            raycastHits[i] = default;
        }
    }

    private void ResetProcess(BoxCollider _collider)
    {
        if (colliderNeedToProcess.Contains(_collider))
        {
            colliderNeedToProcess.Remove(_collider);
        }
        isProcess = false;
    }

    private bool SpawnPosNotInCollider(Vector3 spawnPosition, Vector3 size)
    {
        return Physics.OverlapBox(spawnPosition, size).Length == 0;
    }
}