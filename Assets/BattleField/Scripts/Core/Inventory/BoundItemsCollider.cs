using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundItemsCollider : MonoBehaviour
{
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private List<BoundItem> ItemBases = new();
    [SerializeField] private bool IsDirty = false;
    private Bounds colliderBounds;
    private void Awake()
    {
        boxCollider.isTrigger = true;
        colliderBounds.SetMinMax(ColliderCreator.instance.minSize, ColliderCreator.instance.maxSize);
    }

    public void BoundCollider()
    {
        if (ItemBases == null || ItemBases.Count == 0) return;
        colliderBounds = new Bounds(ItemBases[0].transform.position, Vector3.zero);
        foreach (var t in ItemBases)
        {
            Debug.Log("Bounding: " + t.transform.name);
            colliderBounds.Encapsulate(t.transform.position);
        }
        boxCollider.center = colliderBounds.center;

        Vector3 boundsSize = colliderBounds.size;

        var minSize = ColliderCreator.instance.minSize;
        var maxSize = ColliderCreator.instance.maxSize;

        boundsSize.x = Mathf.Clamp(boundsSize.x, minSize.x, maxSize.x);
        boundsSize.y = Mathf.Clamp(boundsSize.y, minSize.y, maxSize.y);
        boundsSize.z = Mathf.Clamp(boundsSize.z, minSize.z, maxSize.z);

        boxCollider.size = boundsSize;
    }

    public void CanBoundItem(Transform item)
    {
        bool canBoundItem = colliderBounds.Contains(item.position);
        Bounds newBounds = colliderBounds;
        newBounds.Encapsulate(item.position);
    }


    public void AddItemList(BoundItem itemInGame)
    {
        if (ItemBases.Contains(itemInGame)) return;
        ItemBases.Add(itemInGame);
        IsDirty = true;
    }

    public void RemoveItemFromBoundss(BoundItem itemInGame)
    {
        if (!ItemBases.Contains(itemInGame)) return;
        ItemBases.Remove(itemInGame);
        IsDirty = true;
        
        if(ItemBases.Count == 0)
        {
            Debug.Log("Destroy BoundItesmCollider");
            Destroy(gameObject);
        }
    }

    public List<BoundItem>  GetList()
    {
        return ItemBases;
    }
}
