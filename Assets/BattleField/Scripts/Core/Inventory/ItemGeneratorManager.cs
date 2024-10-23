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

    public void CreateFromItemData(ItemData itemData)
    {
        if(CanSpawn(PlayerController.LocalPlayer.GetSoilderPosition(), out var correctSpawnPosition))
        {
            ItemInGame item = Runner.Spawn(itemData.GetItemDataSO().modelPrefab, correctSpawnPosition, Quaternion.identity);
            item.Setup(itemData.GetItemDataSO(),itemData.GetCount());
        }
    }
    public void CreateFromItemData(ItemDataSO itemDataSo, int count, Vector3 spawnPosition)
    {
        if(CanSpawn(spawnPosition, out var correctSpawnPosition))
        {
            ItemInGame item = Runner.Spawn(itemDataSo.modelPrefab, correctSpawnPosition, Quaternion.identity);
            item.Setup(itemDataSo,count);
        }
    }

    private void CreateItemINGame(ItemData itemData, Vector3 correctSpawnPosition)
    {
        
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