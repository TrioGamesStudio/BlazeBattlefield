using Fusion;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
public class DropBox : NetworkBehaviour
{
    [SerializeField] private List<GameObject> itemDropList;
    [SerializeField] private Transform crate_Lid;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private Transform dropTransform;

    [SerializeField] private bool isRandom = false;
    [SerializeField] private int randomItemCount = 3;

    [SerializeField] private AudioClip openAudioClip;

    private bool isOpen = false;
    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        ItemGroundPositioner.instance.SetItemNearGround(boxCollider);
        Close();
    }


    [EditorButton]
    private void Open()
    {
        if (isOpen) return;
        crate_Lid.transform.eulerAngles = new Vector3(-90, 0, 0);
        isOpen = true;
        SoundRequestManager.instance.PlayOneTime(openAudioClip, transform.position);
        if (HasStateAuthority)
        {
            if (isRandom)
            {
                CreateRandomItem();
            }
            else
            {
                CreateItemByList();

            }

            Runner.Despawn(Object);

        }
    }


    private void CreateItemByList()
    {
        foreach (var prefab in itemDropList)
        {
            if (prefab.gameObject.tag != "Item")
            {
                Debug.Log("You need to add correct prefab with tag inside", gameObject);
                return;
            }
            SpawnItemWithRandomPos(prefab);
        }
    }

    private void CreateRandomItem()
    {
        for (int i = 0; i < randomItemCount; i++)
        {
            Runner.Spawn(ItemDatabase.instance.GetRandomItemPrefab(), GetRandomPositionInBoxCollider(boxCollider));
        }
    }

    private void SpawnItemWithRandomPos(GameObject prefab)
    {
        Vector3 randomPosition = GetRandomPositionInBoxCollider(boxCollider);
        Runner.Spawn(prefab, randomPosition);
    }

    [EditorButton]
    private void Close()
    {
        crate_Lid.transform.eulerAngles = new Vector3(0, 0, 0);
        isOpen = false;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Open()
    {
        Open();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward);
    }

    private Vector3 GetRandomPositionInBoxCollider(BoxCollider boxCollider)
    {
        Vector3 center = boxCollider.center;
        Vector3 size = boxCollider.size;

        float randomX = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float randomZ = Random.Range(center.z - size.z / 2, center.z + size.z / 2);

        return boxCollider.transform.TransformPoint(new Vector3(randomX, center.y, randomZ));
    }

}
