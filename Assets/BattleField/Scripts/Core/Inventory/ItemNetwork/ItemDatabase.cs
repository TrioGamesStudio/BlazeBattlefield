using Fusion;
using System;
using System.Collections;
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

    
}
