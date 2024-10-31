using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
[RequireComponent(typeof(Rigidbody))]
public class Test : NetworkBehaviour
{
    private Rigidbody Rigidbody;
    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Rigidbody.isKinematic = true;
        Rigidbody.useGravity = false;
    }

    public override void Spawned()
    {
        base.Spawned();
        if (!HasStateAuthority)
        {
            GetComponent<Test>().enabled = false;
            ItemDatabase.instance.SetPlayerItemSpawnTransform(transform);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CollectItem(other, true);

    }

    private void OnTriggerExit(Collider other)
    {
        CollectItem(other, false);

    }

    private void CollectItem(Collider other, bool showing)
    {
        if (other.CompareTag("BoundItem"))
        {
            var BoundItemsCollider = other.GetComponent<BoundItemsCollider>();
            foreach(var item in BoundItemsCollider.GetList())
            {
                if(item == null)
                {
                    Debug.Log("Have item null when showing in UI", gameObject);
                    continue;
                }

                var runTimeItem = item.GetComponent<RunTimeItem>();
                if (showing)
                {
                    ItemCollectionUI.instance.AddItemUI(runTimeItem);
                }
                else
                {
                    ItemCollectionUI.instance.RemoveItemUI(runTimeItem);
                }
            }
        }

        //if (other.CompareTag("Item") == false) return;
        //var RunTimeItem = other.GetComponent<RunTimeItem>();
        //RunTimeItem.isDisplayedUI = showing;
        
        //if (showing)
        //{
        //    ItemCollectionUI.instance.AddItemUI(RunTimeItem);
        //}
        //else
        //{
        //    ItemCollectionUI.instance.RemoveItemUI(RunTimeItem);
        //}
    }
}
