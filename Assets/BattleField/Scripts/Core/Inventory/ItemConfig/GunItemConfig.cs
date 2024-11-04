using UnityEngine;

[CreateAssetMenu(fileName ="Item_Data",menuName ="Config/Gun")]
public class GunItemConfig : ItemConfig<GunType>
{
    public float fireRate;
    public int maxRounds;
    public AmmoItemConfig ammoUsingType;
    public SlotWeaponIndex slotWeaponIndex;
}

public enum SlotWeaponIndex
{
    Slot_0 = 0,
    Slot_1 = 1,
    Slot_2 = 2,
    Slot_3 = 3,
}