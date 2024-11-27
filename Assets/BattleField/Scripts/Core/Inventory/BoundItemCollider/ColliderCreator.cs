using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class ColliderCreator : MonoBehaviour
{
    public static ColliderCreator instance;
    public Vector3 minSize = new Vector3(1f, 1f, 1f); // Kích thước tối thiểu
    public Vector3 maxSize = new Vector3(10f, 10f, 10f); // Kích thước tối đa
    [SerializeField] private List<BoundItem> processingList = new();
    public LayerMask itemLayerMask;
    public LayerMask boundColliderLayerMask;
    public BoundItemsCollider boundItemPrefab;

    public bool isDebugMode = false;

    private void Awake()
    {
        instance = this;
    }

    public void Add(BoundItem boundItem)
    {
        if (boundItem.CanAddToBound())
        {
            processingList.Add(boundItem);
        }
    }
    private bool isProcessing = false;
    private void Update()
    {
        if (processingList.Count > 0 && isProcessing == false)
        {
            Process();
        }
    }

    private void Process()
    {
        isProcessing = true;

        BoundItem firstBoundItem = processingList[0];
        processingList.Remove(firstBoundItem);

        if (firstBoundItem.CanAddToBound() == false)
        {
            if (isDebugMode)
            {
                Debug.Log($"{firstBoundItem} cannot add to bound", firstBoundItem.gameObject);
            }
            return;
        }
        // check bound collider here
        var BoundResult = Physics.OverlapBox(firstBoundItem.transform.position, minSize, Quaternion.identity, boundColliderLayerMask);

        if (BoundResult.Length > 0)
        {
            foreach (var item in BoundResult)
            {
                if (item.bounds.Contains(firstBoundItem.transform.position))
                {
                    var _boundCollider = item.GetComponent<BoundItemsCollider>();
                    _boundCollider.AddItemList(firstBoundItem);
                    firstBoundItem.IsInBoundCollider = true;
                    firstBoundItem.BoundItemsCollider = _boundCollider;
                    if (isDebugMode)
                    {
                        Debug.Log($"Finded bound {_boundCollider.name} and added {firstBoundItem.name}", firstBoundItem.gameObject);
                    }
                    break;
                }
            }

            if (firstBoundItem.IsInBoundCollider)
            {
                isProcessing = false;
                Debug.Log("Khong can phai kiem tra nua", gameObject);
                return;
            }
        }



        BoundItemsCollider BoundItemsCollider = GetBoundItems();
        //BoundItemsCollider.AddItemList(firstBoundItem);
        //firstBoundItem.isInBoundCollider = true;
        var ItemResult = Physics.OverlapBox(firstBoundItem.transform.position, minSize, Quaternion.identity, itemLayerMask);
        //Debug.Log("Source: " + firstBoundItem.name);
        Debug.Log("Item result: "+ItemResult.Length, gameObject);
        if (ItemResult != null && ItemResult.Length > 0)
        {
            foreach (var item in ItemResult)
            {
                if (item.CompareTag("Item"))
                {
                    var _boundItem = item.GetComponent<BoundItem>();
                    if (_boundItem == null) continue;
                    if (!_boundItem.CanAddToBound()) continue;
                    BoundItemsCollider.AddItemList(_boundItem);
                    _boundItem.IsInBoundCollider = true;
                    _boundItem.BoundItemsCollider = BoundItemsCollider;

                    if (isDebugMode)
                    {
                        Debug.Log($"AddBound: Source {firstBoundItem.name} Add: {_boundItem.name}");
                    }
                    if (processingList.Contains(_boundItem))
                    {
                        processingList.Remove(_boundItem);
                    }
                }
            }
        }
        // destroy if don't have anything
        if (BoundItemsCollider.GetList().Count == 0)
        {
            Destroy(BoundItemsCollider.gameObject);
        }
        else
        {
            BoundItemsCollider.BoundCollider();
        }

        isProcessing = false;
    }


    public BoundItemsCollider GetBoundItems()
    {
        return Instantiate(boundItemPrefab, Vector3.zero, Quaternion.identity, transform);
    }




}