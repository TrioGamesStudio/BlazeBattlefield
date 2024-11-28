using Fusion;
using NaughtyAttributes;
using UnityEngine;
public class ItemGroundPositioner : NetworkBehaviour
{
    public static ItemGroundPositioner instance;
    public LayerMask GroundLayerMask;
    private void Awake()
    {
        instance = this;
    }
    [Header("Testing")]
    public BoxCollider boxCollider;
    [EditorButton]
    private void MethodTest()
    {
        SetItemNearGround(boxCollider);
    }
    private RaycastHit[] raycastHits = new RaycastHit[10];
    public void SetItemNearGround(BoxCollider _collider)
    {
        if (_collider == null) return;
        Ray ray = new Ray(_collider.transform.position, Vector3.down);

        var hits = Physics.RaycastNonAlloc(ray, raycastHits, 100, GroundLayerMask,QueryTriggerInteraction.Collide);
        if(hits == 0)
        {
            Debug.Log("not hit");
            return;
        }
        foreach(var hit in raycastHits)
        {
            if (hit.collider.gameObject.CompareTag("Ground"))
            {
                Vector3 spawnPosition = hit.point + new Vector3(0, _collider.size.y / 2, 0);
                _collider.transform.position = spawnPosition;
                Debug.Log($"Finded grounded: Name{_collider.gameObject.name} : {spawnPosition}",_collider.gameObject);
                break;
            }
        }
    }
 

}