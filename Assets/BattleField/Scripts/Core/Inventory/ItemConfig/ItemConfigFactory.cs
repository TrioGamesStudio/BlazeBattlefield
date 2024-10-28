public static class ItemConfigFactory
{
    public static HealthItemConfig CreateHealthItem(HealingItemType type, string displayName, int maxStack, float healthAmount, float usingItem)
    {
        HealthItemConfig config = new();
        config.displayName = displayName;
        config.maxStack = maxStack;
        config.SubItemType = type;
        config.ItemType = ItemType.Health;
        config.usingTime = usingItem;
        config.healthAmount = healthAmount;
        return config;
    }

    public static AmmoItemConfig CreateAmmoConfig(AmmoType type, string displayName, int maxStack)
    {
        AmmoItemConfig config = new();
        config.displayName = displayName;
        config.maxStack = maxStack;
        config.SubItemType = type;
        config.ItemType = ItemType.Ammo;
        return config;
    }
}

