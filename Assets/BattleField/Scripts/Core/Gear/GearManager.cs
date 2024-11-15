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
    }
}
