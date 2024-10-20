using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item") == false) return;
        Debug.Log("Enter");
        ItemCollectionUI.instance.AddItem(other.GetComponent<ItemInGame>());
        // if (other.CompareTag("ItemBound") == false) return;
        // Debug.Log("Trigger",gameObject);
        // ItemCollectionUI.instance.BindItems(other.GetComponent<BoundItems>());
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item") == false) return;
        Debug.Log("Exit");
        ItemCollectionUI.instance.RemoveItem(other.GetComponent<ItemInGame>());
        // if (other.CompareTag("ItemBound") == false) return;
        // Debug.Log("Exit",gameObject);
        // ItemCollectionUI.instance.BindItems(null);
    }
}
