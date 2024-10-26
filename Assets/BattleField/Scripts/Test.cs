using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("Item") == false) return;
        //Debug.Log("Enter");
        //var itemInGame = other.GetComponent<ItemInGame>();
        //itemInGame.IsDisplayedInUI = true;

        if (other.CompareTag("Item") == false) return;
        var RunTimeItem = other.GetComponent<RunTimeItem>();
        //InteractItem(RunTimeItem);
        RunTimeItem.isDisplayedUI = true;
        ItemCollectionUI.instance.AddItemUI(RunTimeItem);

    }
    private void OnTriggerExit(Collider other)
    {
        //if (other.CompareTag("Item") == false) return;
        //Debug.Log("Exit");
        //var itemInGame = other.GetComponent<ItemInGame>();
        //itemInGame.IsDisplayedInUI = false;
        //ItemCollectionUI.instance.RemoveItemUI(itemInGame);

        if (other.CompareTag("Item") == false) return;
        var RunTimeItem = other.GetComponent<RunTimeItem>();
        RunTimeItem.isDisplayedUI = false;
        //InteractItem(RunTimeItem);
        ItemCollectionUI.instance.RemoveItemUI(RunTimeItem);

    }
    
}
