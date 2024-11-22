using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ItemDatabase : NetworkBehaviour
{
    public static ItemDatabase instance;
    [SerializeField] public ItemConfigDatabase ItemConfigDatabase;
    [SerializeField] private ItemPrefabDatabase ItemPrefabDatabase;

    public Transform PlayerObject;

    private void Awake()
    {
        instance = this;
        ItemPrefabDatabase.Convert();
    }
    public override void Spawned()
    {
        base.Spawned();
    }

    public GameObject GetItemPrefab(ItemType key1, Enum key2)
    {
        return ItemPrefabDatabase.GetItemPrefab(key1, key2);
    }



    // Use by inventory spawning
    public void InventoryItemToWorld(InventoryItem inventoryItem, int newAmount)
    {
        var key1 = inventoryItem.ItemType;
        var key2 = inventoryItem._SubItemEnum;

        CreateItemInWorld(newAmount, key1, key2, PlayerObject.position);
    }

    // use for spawn by item config
    public void GunConfigToWorld(GunItemConfig config, int quantity)
    {
        CreateItemInWorld(quantity, config.ItemType, config.SubItemType, PlayerObject.position);
    }

    public void ArmorConfigToWorld(ArmorConfig config, float durability)
    {
        var item = CreateItemInWorld(1, config.ItemType, config.SubItemType, PlayerObject.position);
        item.SetCustomData(GearManager.DurabilityKey, durability);
    }

    private ItemDataEnum CreateItemInWorld(int newAmount, ItemType key1, Enum key2, Vector3 position)
    {
        var itemPrefab = GetItemPrefab(key1, key2);
        if (itemPrefab == null) return null;

        var item = Runner.Spawn(itemPrefab, position).GetComponent<ItemDataEnum>();
        item.SetQuantity(newAmount);

        return item;
    }

    public NetworkObject SpawnItem(GameObject prefab, Vector3 position, string _tag)
    {
        //var position = isLocal ? weaponHoldersLocal[index].position : weaponHoldersRemote[index].position;
        var networkObject = Runner.Spawn(prefab, position, Quaternion.Euler(0, 0, 0), null, (runner, obj) =>
        {
            obj.GetComponent<BoundItem>().allowAddToCollider = false;
            obj.GetComponent<TagObjectHandler>().SetTag_RPC(_tag);
        });
        Debug.Log("Start set parent", gameObject);
        //RPC_SetParentWeapon(networkObject, isLocal, index);
        return networkObject;
    }
    Dictionary<ItemRarity, float> rarityChances = new Dictionary<ItemRarity, float>
{
    { ItemRarity.Common, 60f },
    { ItemRarity.Rare, 30f },
    { ItemRarity.Epic, 8f }
};
    public GameObject GetRandomItemPrefab()
    {
        var rarity = GetRandomItemRarity();
        return ItemPrefabDatabase.GetRandomItemPrefabByRarity(rarity);
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
}
