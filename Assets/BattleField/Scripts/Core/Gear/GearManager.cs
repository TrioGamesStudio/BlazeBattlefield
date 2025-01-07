using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearManager : MonoBehaviour
{
    public static string DurabilityKey = "durability";

    public static GearManager instance;
    [SerializeField] private ArmorConfig currentArmorEquip;
    [SerializeField] private float currentArmorDurability;
    [SerializeField] public GearActive gearActive;
    private void Awake()
    {
        instance = this;
    }
    public void AddArmor(ArmorConfig config,float currentDurability)
    {
        if(currentArmorEquip != null)
            ItemDatabase.instance.ArmorConfigToWorld(config, currentDurability);
        currentArmorEquip = config;
        currentArmorDurability = currentDurability;
        EquipToPlayer();
    }

    private void EquipToPlayer()
    {
        var prefab = ItemDatabase.instance.GetItemPrefab(ItemType.Armor, currentArmorEquip.SubItemType);

        if(prefab == null)
        {
            Debug.LogError("Item Prefab is null", gameObject);
            return;
        }

        var networkObject = ItemDatabase.instance.SpawnItem(prefab, NetworkPlayer.Local.transform.position, "Item");
        var armor = networkObject.GetComponent<ArmorItem>();

    }
}
