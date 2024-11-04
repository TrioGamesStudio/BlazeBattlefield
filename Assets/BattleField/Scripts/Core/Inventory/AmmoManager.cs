using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    [Serializable]
    public class AmmoData
    {
        public AmmoData(AmmoType ammoType)
        {
            Type = ammoType;
            totalAmmo = 0;
        }
        public AmmoType Type;
        public int totalAmmo;
    }

    public static AmmoManager instance;
    [SerializeField] private int totalAmount;
    [SerializeField] private List<AmmoData> ammoDatas = new();


    private void Awake()
    {
        instance = this;
        Init();
    }
    private void Init()
    {
        ammoDatas = new()
        {
            new AmmoData(AmmoType.Ammo556),
            new AmmoData(AmmoType.Ammo762),
            new AmmoData(AmmoType.Ammo9mm),
            new AmmoData(AmmoType.Ammo12Gauge)
        };
    }

    public void AddAmmo(InventoryItem inventory, int quantity)
    {
        HandlerAmmo(inventory, true, quantity);
    }
    public void RemoveAmmo(InventoryItem inventory, int quantity)
    {
        HandlerAmmo(inventory, false, quantity);
    }

    private void HandlerAmmo(InventoryItem inventoryItem, bool isAdd, int quantity)
    {
        if (inventoryItem.ItemType != ItemType.Ammo) return;
        foreach (var item in ammoDatas)
        {
            if (item.Type.Equals(inventoryItem._SubItemEnum))
            {
                if (isAdd)
                {
                    item.totalAmmo += quantity;
                }
                else
                {
                    item.totalAmmo -= quantity;
                }
            }
        }

    }



}
