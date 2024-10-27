public class ItemBackpackUI : ItemCollectUI
{
    public ItemBPData _ItemBPData;
    public struct ItemBPData
    {
        public ItemType itemType;
        public int quantity;
        public int enumIndex;
    }
    public void SetItemBPData(ItemType itemType, int quantity, int enumIndex)
    {
        _ItemBPData.itemType = itemType;
        _ItemBPData.quantity = quantity;
        _ItemBPData.enumIndex = enumIndex;
    }
}
