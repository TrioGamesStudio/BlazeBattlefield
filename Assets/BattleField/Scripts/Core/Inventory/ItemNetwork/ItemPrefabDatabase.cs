using Fusion;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

[CreateAssetMenu(fileName = "ItemPrefabDatabase", menuName = "Config/ItemPrefabDatabase")]
public class ItemPrefabDatabase: ScriptableObject
{

    [SerializeField] private List<GameObject> gameObjects;
    private Dictionary<(ItemType, Enum), GameObject> bigData = new();
    private Dictionary<ItemRarity, List<ItemDataEnum>> itemRarityDatas = new();
    public GameObject GetItemPrefab(ItemType key1, Enum key2)
    {
        if (bigData.TryGetValue((key1, key2), out var prefab))
        {
            return prefab;
        }
        Debug.LogError($"This item {key1.ToString()} {key2.ToString()} is null");
        return null;
    }
#if UNITY_EDITOR

    [Button]
    private void GetAsset()
    {
        gameObjects.Clear();
        gameObjects = LoadAssetHeplder.GetListTypeInAssets<GameObject>(LoadAssetHeplder.SearchType.Label, "Item");
    }
#endif
    [Button]
    public void Convert()
    {
        itemRarityDatas.Clear();
        itemRarityDatas.Add(ItemRarity.Common, new());
        itemRarityDatas.Add(ItemRarity.Rare, new());
        itemRarityDatas.Add(ItemRarity.Epic, new());

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
        if (itemRarityDatas.TryGetValue(itemEnum.GetItemRarity(), out var list))
        {
            if (list.Contains(itemEnum) == false)
            {
                list.Add(itemEnum);
            }
        }

    }
    [Button]
    private void Test()
    {
        Debug.Log(GetRandomItemPrefabByRarity(ItemRarity.Common));
    }

    public GameObject GetRandomItemPrefabByRarity(ItemRarity rarity)
    {
        if (itemRarityDatas.TryGetValue(rarity, out var list))
        {
            int totalWeight = list.Sum(item => item.GetItemWeight());

            int randomWeight = UnityEngine.Random.Range(0, totalWeight);

            int currentWeight = 0;
            foreach (var item in list)
            {
                currentWeight += item.GetItemWeight();
                if (randomWeight < currentWeight)
                {
                    if(bigData.TryGetValue((item.GetItemType(),item.GetSubItemType()),out var _item))
                    {
                        return _item;
                    }
                }
            }
        }
        return null;
    }
    private ItemRarity GetRandomItemRarity()
    {
        float randomValue = UnityEngine.Random.Range(0f, 100f);
        float cumulativeChance = 0f;
        foreach (var rarity in rarityChances)
        {
            cumulativeChance += rarity.Value;
            if (randomValue <= cumulativeChance)
                return rarity.Key;
        }
        return ItemRarity.Common;
    }
    Dictionary<ItemRarity, float> rarityChances = new Dictionary<ItemRarity, float>
{
    { ItemRarity.Common, 60f },
    { ItemRarity.Rare, 30f },
    { ItemRarity.Epic, 8f }
};
}
