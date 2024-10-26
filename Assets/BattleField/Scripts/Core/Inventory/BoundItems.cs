using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundItems : MonoBehaviour
{
    public BoxCollider boxCollider;
    public List<ItemInGame> ItemBases = new();
    public bool IsDirty = false;
    private void Awake()
    {
        boxCollider.isTrigger = true;
    }

    public void BoundCollider()
    {
        // if (ItemBases == null || ItemBases.Count == 0) return;
        // Bounds bounds = new Bounds(ItemBases[0].model.transform.position, Vector3.zero);
        // foreach (var t in ItemBases)
        // {
        //     Debug.Log("Bounding: " + t.model.transform.name);
        //     bounds.Encapsulate(t.model.transform.position);
        // }
        // boxCollider.center = bounds.center;

        // Tính toán và giới hạn kích thước
        // Vector3 boundsSize = bounds.size;
        //
        // var minSize = ColliderCreator.instance.minSize;
        // var maxSize = ColliderCreator.instance.maxSize;

        // boundsSize.x = Mathf.Clamp(boundsSize.x, minSize.x, maxSize.x);
        // boundsSize.y = Mathf.Clamp(boundsSize.y, minSize.y, maxSize.y);
        // boundsSize.z = Mathf.Clamp(boundsSize.z, minSize.z, maxSize.z);
        //
        // // Đặt kích thước đã giới hạn cho BoxCollider
        // boxCollider.size = boundsSize;
    }


    // public void AddItemList(ItemInGame itemInGame)
    // {
    //     if (ItemBases.Contains(itemInGame)) return;
    //     ItemBases.Add(itemInGame);
    //     IsDirty = true;
    // }
    //
    // public void RemoveItemFromBoundss(ItemInGame itemInGame)
    // {
    //     if (!ItemBases.Contains(itemInGame)) return;
    //     ItemBases.Remove(itemInGame);
    //     IsDirty = true;
    // }
}
