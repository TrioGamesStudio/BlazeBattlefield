using Fusion;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemGeneratorManager : NetworkBehaviour
{
    public static ItemGeneratorManager instance;
    private Dictionary<string,ItemDataSO> itemDict = new();
    [SerializeField] private List<ItemDataSO> itemDataList = new();
    private void Awake()
    {
        instance = this;
    }
    public ItemInGame CreateItemInWorld(ItemDataSO itemDataSO)
    {
        ItemInGame item = Runner.Spawn(itemDataSO.modelPrefab, RandomPosition(), Quaternion.identity);
        item.Setup(itemDataSO, Random.Range(1, 5));
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
        ItemInGame item = Runner.Spawn(itemDataSO.modelPrefab, RandomPosition(), Quaternion.identity);
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

}