public static class ItemConfigFactory
{
    public static HealthItemConfig CreateHealthItem(HealthItemConfig config,HealingItemType type, string displayName, int maxStack, float healthAmount, float usingItem)
    {
        config.displayName = displayName;
        config.maxStack = maxStack;
        config.SubItemType = type;
        config.ItemType = ItemType.Health;
        config.usingTime = usingItem;
        config.healthAmount = healthAmount;
        return config;
    }

    public static AmmoItemConfig CreateAmmoConfig(AmmoItemConfig config,AmmoType type, string displayName, int maxStack)
    {
        config.displayName = displayName;
        config.maxStack = maxStack;
        config.SubItemType = type;
        config.ItemType = ItemType.Ammo;
        return config;
    }
}

