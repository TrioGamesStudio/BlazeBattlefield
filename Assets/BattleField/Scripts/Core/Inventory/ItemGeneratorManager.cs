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
        foreach (var item in itemDataList)
        {
            itemDict.Add(item.name,item);
        }
    }

    public ItemDataSO GetItemDataSO(string name)
    {
        if (itemDict.TryGetValue(name, out var value))
        {
            return value;
        }

        return null;
    }
    
    private Vector3 RandomPosition()
    {
        return new Vector3(Random.Range(-1f, 1), 0, Random.Range(-1f, 1));
    }
    private void OnDestroy()
    {
        itemDict.Clear();
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

    public void CreateFromItemData(ItemLocalData itemData)
    {
        if(CanSpawn(NetworkPlayer.Local.transform.position, out var correctSpawnPosition))
        {
            ItemInGame item = Runner.Spawn(itemData.ItemData.modelPrefab, correctSpawnPosition, Quaternion.identity);
            item.SetItemNetworkData(CreateItemDataNetwork(itemData.ItemData.name,itemData.CurrentQuantity));
        }
    }
    public void CreateFromItemData(ItemDataSO itemDataSo, int count, Vector3 spawnPosition)
    {
        if(CanSpawn(spawnPosition, out var correctSpawnPosition))
        {
            ItemInGame item = Runner.Spawn(itemDataSo.modelPrefab, correctSpawnPosition, Quaternion.identity);
            item.SetItemNetworkData(CreateItemDataNetwork(itemDataSo.name,count));
        }
    }

    private ItemDataNetwork CreateItemDataNetwork(string name,int count)
    {
        ItemDataNetwork itemDataNetwork = new ItemDataNetwork();
        itemDataNetwork.ItemDataSOName = name;
        itemDataNetwork.CurrentCount = count;
        return itemDataNetwork;
    }
  

    public void CreateRandomItemFromSource()
    {
        ItemDataSO itemDataSO = itemDataList[Random.Range(0, itemDataList.Count)];
        if (itemDataSO == null)
        {
            Debug.Log("Item Data SO null", gameObject);
            return;
        }
        CreateFromItemData(itemDataSO, Random.Range(1, itemDataSO.maxCountPerStack),RandomPosition());
    }
}