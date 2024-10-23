using Fusion;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemGeneratorManager : NetworkBehaviour
{
    public static ItemGeneratorManager instance;
    private Dictionary<string, ItemDataSO> itemDict = new();
    [SerializeField] private List<ItemDataSO> itemDataList = new();
    public LayerMask groundLayers;
    private void Awake()
    {
        instance = this;
    }
    public ItemInGame CreateRandomCountItemInWorld(ItemDataSO itemDataSO)
    {
        ItemInGame item = CreateItem(itemDataSO, RandomPosition());
        item.Setup(itemDataSO, Random.Range(1, itemDataSO.maxCountPerStack - 1));
        return item;
    }
    public ItemInGame CreateItemRandomPositionInWorld(ItemDataSO itemDataSO, int count)
    {
        ItemInGame item = CreateItem(itemDataSO, RandomPosition());
        item.Setup(itemDataSO, count);
        return item;
    }
    public ItemInGame CreateItemInWorld(ItemDataSO itemDataSO, Vector3 position, int count)
    {

        ItemInGame item = CreateItem(itemDataSO, position);
        item.Setup(itemDataSO, count);
        return item;
    }

    public ItemInGame CreateRandomItemInWorld()
    {
        ItemDataSO itemDataSO = itemDataList[Random.Range(0, itemDataList.Count)];
        if (itemDataSO == null)
        {
            Debug.Log("Item Data SO null", gameObject);
            return null;
        }
        ItemInGame item = CreateItem(itemDataSO, RandomPosition());
        item.Setup(itemDataSO, Random.Range(1, 5));
        return item;
    }
    private Vector3 RandomPosition()
    {
        return new Vector3(Random.Range(-1f, 1), 0, Random.Range(-1f, 1));
    }
    private void OnDestroy()
    {
        itemDict.Clear();
    }
    private ItemInGame CreateItem(ItemDataSO itemDataSO, Vector3 position)
    {
        if(CanSpawn(position, out var correctSpawnPosition))
        {
            ItemInGame item = Runner.Spawn(itemDataSO.modelPrefab, correctSpawnPosition, Quaternion.identity);
            return item;
        }
        return null;
    }
    private void OnDrawGizmos()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if(Physics.Raycast(ray,out var hit, 100))
        {
            Gizmos.DrawSphere(hit.point, .5f);
            Gizmos.color = Color.red;
        }
        Gizmos.DrawRay(ray);
        Gizmos.color = Color.cyan;
    }

    private bool CanSpawn(Vector3 spawnPosition, out Vector3 correctSpawnPosition)
    {
        correctSpawnPosition = Vector3.zero;

        Ray ray = new Ray(spawnPosition, Vector3.down);
        if (Physics.Raycast(ray, out var hit, 100, groundLayers))
        {
            correctSpawnPosition = hit.point;
            return true;
        }
        return false;
    }
}