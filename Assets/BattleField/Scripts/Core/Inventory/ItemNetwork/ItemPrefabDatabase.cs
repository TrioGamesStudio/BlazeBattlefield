using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemPrefabDatabase", menuName = "Config/ItemPrefabDatabase")]
public class ItemPrefabDatabase: ScriptableObject
{

    [SerializeField] private List<GameObject> gameObjects;
    private Dictionary<(ItemType, Enum), GameObject> bigData = new();
    public GameObject GetItemPrefab(ItemType key1, Enum key2)
    {
        if (bigData.TryGetValue((key1, key2), out var prefab))
        {
            return prefab;
        }
        Debug.LogError($"This item {key1.ToString()} {key2.ToString()} is null");
        return null;
    }
  
    public void Convert()
    {
        foreach (var item in gameObjects)
        {
            CastToGeneralItem(item);
        }
    }

    private void CastToGeneralItem(GameObject item)
    {
        var itemEnum = item.GetComponent<ItemDataEnum>();
        if (itemEnum == null)
            return;
        var key1 = itemEnum.GetItemType();
        var key2 = itemEnum.GetSubItemType();

        if (!bigData.ContainsKey((key1, key2)))
        {
            bigData.Add((key1, key2), item);
        }
        else
        {
            bigData[(key1, key2)] = item;
        }
    }
}
