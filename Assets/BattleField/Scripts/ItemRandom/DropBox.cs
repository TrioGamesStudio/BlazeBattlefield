using Fusion;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using DG.Tweening;
public class DropBox : NetworkBehaviour
{
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private AudioClip openAudioClip;
    [SerializeField] private Transform openBoxTransform;
    [Header("Drop Settings")]
    [SerializeField] private List<GameObject> itemDropList;
    [SerializeField] private bool allowRandomItem = false;
    [SerializeField] private int randomItemCount = 3;
    [SerializeField] private float delayDestroyTime = .5f;
    [SerializeField] private Vector3 dropBoundVector3;
    private bool isOpen = false;


    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        //ItemGroundPositioner.instance.SetItemNearGround(boxCollider);
        Close();
    }


    [EditorButton]
    private void Open()
    {
        if (isOpen) return;
        //crate_Lid.transform.eulerAngles = new Vector3(-90, 0, 0);
        isOpen = true;
        SoundRequestManager.instance.PlayOneTime(openAudioClip, transform.position);
        openBoxTransform.DOShakePosition(.3f).OnComplete(() =>
        {
            if (allowRandomItem && HasStateAuthority)
            {
                CreateRandomItem();
            }

            openBoxTransform.DOScale(Vector3.zero, .3f).SetEase(Ease.InOutBack);
        });
        if (HasStateAuthority)
            Invoke(nameof(DestroyItemDelay), 0.6f);
    }

    private void DestroyItemDelay()
    {
        Runner.Despawn(Object);
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
            var prefab = ItemDatabase.instance.GetRandomItemPrefab();
            if (prefab.TryGetComponent(out GunItem gunPrefab))
            {
                var ammoPrefab = ItemDatabase.instance.GetItemPrefab(ItemType.Ammo, gunPrefab.config.ammoUsingType.SubItemType);
                Runner.Spawn(ammoPrefab, GetRandomPositionInBoxCollider(boxCollider));
            }
            Runner.Spawn(prefab, GetRandomPositionInBoxCollider(boxCollider));
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
        //crate_Lid.transform.eulerAngles = new Vector3(0, 0, 0);
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
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, dropBoundVector3);
    }

    private Vector3 GetRandomPositionInBoxCollider(BoxCollider boxCollider)
    {
        Vector3 center = boxCollider.center;
        Vector3 size = dropBoundVector3;

        float randomX = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float randomZ = Random.Range(center.z - size.z / 2, center.z + size.z / 2);

        return boxCollider.transform.TransformPoint(new Vector3(randomX, center.y, randomZ));
    }

}
