using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTriggerCollect : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Item")) return;
        if (other.TryGetComponent(out BoundItem boundItem)
            && boundItem.IsInBoundCollider == false)
        {
            Debug.Log($"Setup {other.gameObject.name}");
            boundItem.Setup();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("BoundItem")) return;
        if (other.TryGetComponent(out BoundItemsCollider boundItemsCollider))
        {
            boundItemsCollider.ClearItemBound();
        }
    }

    private void ProcessBoundItemCollider(Collider other, bool isEnter)
    {
        if (!other.gameObject.CompareTag("BoundItem")) return;
        if (other.TryGetComponent(out BoundItemsCollider boundItemsCollider))
        {
            if (isEnter)
            {
                // remove it from counter delete
            }
            else
            {
                boundItemsCollider.ClearItemBound();
                // add it to counter delete
            }

        }
    }

}


