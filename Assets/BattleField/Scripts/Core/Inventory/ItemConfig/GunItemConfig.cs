using UnityEngine;

[CreateAssetMenu(fileName ="Item_Data",menuName ="Config/Gun")]
public class GunItemConfig : ItemConfig<GunType>
{
    public float fireRate;
    public int maxRounds;
    public AmmoType ammoUsingType;
}