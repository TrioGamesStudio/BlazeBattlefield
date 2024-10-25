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

    private Dictionary<AmmoType, int> ammoInfomation;
    private Dictionary<HealingItemType, int> healthInfomation;
    private void Init()
    {
        ammoInfomation = new()
        {
            {AmmoType.Ammo556,0 },
            {AmmoType.Ammo762,0 },
            {AmmoType.Ammo9mm,0 },
            {AmmoType.ShotgunShell,0 },
            {AmmoType.Ammo12Gauge,0 },
        };

        healthInfomation = new()
        {
            {HealingItemType.Bandage,0 },
            {HealingItemType.FirstAidKit,0 },
            {HealingItemType.Medkit,0 },
        };
    }
    public void AddAmmo(AmmoType ammoType, int quantity)
    {
        ammoInfomation[ammoType] += quantity;
        Debug.Log($"Current ammo: {ammoType.ToString()} couting is : {ammoInfomation[ammoType]}!!");
    }

    public void RemoveAmmo(AmmoType ammoType, int quantity)
    {
        ammoInfomation[ammoType] -= quantity;
    }

    public void AddHealth(HealingItemType heallingItemType, int quantity)
    {
        healthInfomation[heallingItemType] += quantity;
        Debug.Log($"Current ammo: {heallingItemType.ToString()} couting is : {healthInfomation[heallingItemType]}!!");
    }

    public void RemoveHealth(HealingItemType heallingItemType, int quantity)
    {
        healthInfomation[heallingItemType] -= quantity;
    }
}

public enum HealingItemType
{
    None,          
    Bandage,       
    FirstAidKit,   
    Medkit,        
    EnergyDrink,   
    Painkiller     
}