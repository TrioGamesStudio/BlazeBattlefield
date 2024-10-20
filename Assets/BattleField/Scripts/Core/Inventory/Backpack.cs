using System.Collections;
using System.Collections.Generic;
using Fusion;
using NaughtyAttributes;
using UnityEngine;

public class Backpack : MonoBehaviour
{
    public static Backpack instance;
    [SerializeField] private List<ItemData> itemVen = new();
    private void Awake()
    {
        instance = this;
    }

    public void AddItemToInventory(ItemData itemData)
    {
        itemVen.Add(itemData);
    }

    public void Drop(ItemData itemData)
    {
        itemVen.Remove(itemData);
    }
    public bool CanCollect()
    {
        return true;
    }
}


public enum ItemType
{
    Gun,
    Melee,
    Ammo,
    Health,
    Grenade,
}

