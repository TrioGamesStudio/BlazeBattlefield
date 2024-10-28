using System;
using System.Collections.Generic;
using UnityEngine;
public class StorageManager : MonoBehaviour
{

    public static StorageManager instance;
    private void Awake()
    {
        instance = this;
        Init();
    }

    private Dictionary<AmmoType, int> ammoInformation;
    private Dictionary<HealingItemType, int> healthInformation;

    private void Init()
    {
        ammoInformation = new()
        {
            {AmmoType.Ammo556, 0},
            {AmmoType.Ammo762, 0},
            {AmmoType.Ammo9mm, 0},
            {AmmoType.Ammo12Gauge, 0},
        };

        healthInformation = new()
        {
            {HealingItemType.Bandage, 0},
            {HealingItemType.FirstAidKit, 0},
            {HealingItemType.Medkit, 0},
        };
    }


    private void UpdateItem<T>(Dictionary<T, int> itemDictionary, T itemType, int quantity, bool isAdding)
    {
        if (itemDictionary.ContainsKey(itemType))
        {
            itemDictionary[itemType] += isAdding ? quantity : -quantity;
            Debug.Log($"Current {typeof(T).Name}: {itemType} count is: {itemDictionary[itemType]}");
        }
        else
        {
            Debug.LogWarning($"{typeof(T).Name} type {itemType} not found in dictionary.");
        }

        if (itemDictionary[itemType] <= 0)
        {
            Debug.LogError("item is zero");
        }
    }

    public void UpdateAmmo(AmmoType ammoType, int quantity, bool isAdding)
    {
        UpdateItem(ammoInformation, ammoType, quantity, isAdding);
    }

    public void UpdateHealth(HealingItemType healingItemType, int quantity, bool isAdding)
    {
        UpdateItem(healthInformation, healingItemType, quantity, isAdding);
    }

}

public enum HealingItemType
{
    None = 0,
    Bandage = 5,
    FirstAidKit = 10,
    Medkit = 15,
}
