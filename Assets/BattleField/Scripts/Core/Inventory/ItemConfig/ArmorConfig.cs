using UnityEngine;

[CreateAssetMenu(fileName = "Item_Data", menuName = "Config/Armor")]
public class ArmorConfig : ItemConfig<ArmorType>
{
    public float durability;
    public float damageResitant;
}
