using System.Collections.Generic;

public static class ItemConfigDefaultSettings
{
    public static List<HealthItemConfig> LoadHealth()
    {
        var itemConfigs = new List<HealthItemConfig>()
        {
            ItemConfigFactory.CreateHealthItem(HealingItemType.Bandage,"Bandage",10,15,.8f),
            ItemConfigFactory.CreateHealthItem(HealingItemType.FirstAidKit,"First Aid Kit",5,70,1.5f),
            ItemConfigFactory.CreateHealthItem(HealingItemType.Medkit,"Med Kit",5,100,2f),
        };
        return itemConfigs;
    }

    public static List<AmmoItemConfig> LoadAmmo()
    {
        var itemConfigs = new List<AmmoItemConfig>()
        {
            ItemConfigFactory.CreateAmmoConfig(AmmoType.Ammo556,"5.56mm Ammo",30),
           ItemConfigFactory.CreateAmmoConfig(AmmoType.Ammo9mm,"0.9mm Ammo",30),
            ItemConfigFactory.CreateAmmoConfig(AmmoType.Ammo762,"7.62mm Ammo",30),
            ItemConfigFactory.CreateAmmoConfig(AmmoType.Ammo12Gauge,"12 Gauge Ammo",30),
        };
        return itemConfigs;
    }

}

