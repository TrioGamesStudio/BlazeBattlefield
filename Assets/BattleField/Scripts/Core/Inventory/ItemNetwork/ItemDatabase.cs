using Fusion;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(-98)]
public class ItemDatabase : NetworkBehaviour
{
    public static ItemDatabase instance;
    [SerializeField] public ItemConfigDatabase ItemConfigDatabase;
    [SerializeField] private ItemPrefabDatabase ItemPrefabDatabase;

    public Transform PlayerObject;
    [SerializeField] public List<NetworkObject> itemList = new();
    private void Awake()
    {
        instance = this;
        ItemPrefabDatabase.Convert();
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

        var networkobject = Runner.Spawn(itemPrefab, position);
        var itemDataEnum = networkobject.GetComponent<ItemDataEnum>();
      
        itemDataEnum.SetQuantity(newAmount);
      
        return itemDataEnum;
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

    public GameObject GetRandomItemPrefab()
    {
        return ItemPrefabDatabase.GetRandomItemPrefabByRarity();
    }

    // duplicate code
    public void RequestStateAuthority()
    {
        if (Object == null)
        {
            Debug.Log($"///Object is null or destroyed on {gameObject.name}");
            return;
        }

        if (!Object.HasStateAuthority)
        {
            try
            {
                Object.RequestStateAuthority();
                Debug.Log($"///Requesting state authority for bot {gameObject.name}.");
            }
            catch (Exception ex)
            {
                Debug.Log($"///Failed to request state authority: {ex.Message}");
            }
            TestRequest();
        }
        else
        {
            Debug.Log("///Object already has state authority.");
        }
    }

    [EditorButton] 
    private void TestRequest()
    {
        foreach (var item in itemList)
        {
            item.RequestStateAuthority();
        }
    }


    public void AddItem(NetworkObject networkObject)
    {
        if (!itemList.Contains(networkObject))
        {
            itemList.Add(networkObject);
        }
    }

    public void RemoveItem(NetworkObject networkObject)
    {
        if (itemList.Contains(networkObject))
        {
            itemList.Remove(networkObject);
        }
    }
}
