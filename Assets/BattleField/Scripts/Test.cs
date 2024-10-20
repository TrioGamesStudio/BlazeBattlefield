using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item") == false) return;
        ItemCollectionUI.instance.AddItem(other.gameObject);
        // if (other.CompareTag("ItemBound") == false) return;
        // Debug.Log("Trigger",gameObject);
        // ItemCollectionUI.instance.BindItems(other.GetComponent<BoundItems>());
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item") == false) return;
        ItemCollectionUI.instance.RemoveItem(other.gameObject);
        // if (other.CompareTag("ItemBound") == false) return;
        // Debug.Log("Exit",gameObject);
        // ItemCollectionUI.instance.BindItems(null);
    }
}
