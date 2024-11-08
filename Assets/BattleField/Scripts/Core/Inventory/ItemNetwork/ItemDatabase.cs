using Fusion;
using System;
using System.Collections;
using UnityEngine;
public class ItemDatabase : NetworkBehaviour
{
    public static ItemDatabase instance;
    [SerializeField] private ItemConfigDatabase ItemConfigDatabase;
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


    public ItemConfig<Enum> GetItemConfig(ItemType itemType, Enum subItemType)
    {
        return ItemConfigDatabase.FindItem(itemType, subItemType);
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


    private void CreateItemInWorld(int newAmount, ItemType key1, Enum key2, Vector3 position)
    {
        var itemPrefab = GetItemPrefab(key1, key2);
        if (itemPrefab == null) return;

        var item = Runner.Spawn(itemPrefab, position).GetComponent<ItemDataEnum>();
        item.SetQuantity(newAmount);
    }

    
}
