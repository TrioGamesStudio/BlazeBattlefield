using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
[RequireComponent(typeof(Rigidbody))]
public class PlayerCollectManager : NetworkBehaviour, IStateAuthorityChanged
{
    [SerializeField] private bool isPlayer;
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
            gameObject.SetActive(false);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPlayer)
            CollectItem(other, true);
        TriggerDropBox(other);

    }

    private void OnTriggerExit(Collider other)
    {
        CollectItem(other, false);
    }

    private void TriggerDropBox(Collider other)
    {
        if(other.TryGetComponent(out DropBox dropBox))
        {
            dropBox.RPC_Open();
        }
    }

    private void CollectItem(Collider other, bool showing)
    {
        if (other.CompareTag("BoundItem"))
        {
            var BoundItemsCollider = other.GetComponent<BoundItemsCollider>();
            if (showing)
            {
                BoundItemsCollider.OnChangedList = OnChangeShowList;
            }
            else
            {
                BoundItemsCollider.OnChangedList = null;
            }
            OnShowItemList(showing, BoundItemsCollider.GetList());
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
    private void OnChangeShowList(List<BoundItem> list)
    {
        OnShowItemList(true, list);
    }
    private void OnShowItemList(bool showing, List<BoundItem> list)
    {
        foreach (var item in list)
        {
            if (item == null)
            {
                Debug.Log("Have item null when showing in UI", gameObject);
                continue;
            }

            var runTimeItem = item.GetComponent<IRunTimeItem>();
            if (showing)
            {
                runTimeItem.isDisplayedUI = true;
                ItemCollectionUI.instance.AddItemUI(runTimeItem);
            }
            else
            {
                runTimeItem.isDisplayedUI = false;
                ItemCollectionUI.instance.RemoveItemUI(runTimeItem);
            }
        }
    }

    public void StateAuthorityChanged()
    {
        if (!HasStateAuthority)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
