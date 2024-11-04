using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    

    public static AmmoManager instance;
    [SerializeField] private int totalAmount;
    [SerializeField] private List<AmmoItemConfig> ammoConfigs;
    private Dictionary<Enum, AmmoItemConfig> ammoConfigDictionary;
    private void Awake()
    {
        instance = this;
        Init();
    }

    private void Init()
    {
        ammoConfigDictionary = new();
        foreach (var item in ammoConfigs)
        {
            ammoConfigDictionary.Add(item.SubItemType, item);
        }
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
       
        if(ammoConfigDictionary.TryGetValue(inventoryItem._SubItemEnum,out var ammoConfig))
        {
            if (isAdd)
            {
                ammoConfig.ChangeTotalAmmo(quantity);
            }
            else
            {
                ammoConfig.ChangeTotalAmmo(-quantity);
            }
        }
    }



}
