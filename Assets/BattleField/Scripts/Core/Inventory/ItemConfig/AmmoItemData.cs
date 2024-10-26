using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Ammo_Data", menuName = "Item/Ammo_Data")]
public class AmmoItemData : ItemConfigSettings<AmmoType, AmmoItemConfig>
{

}
[Serializable]
public class AmmoItemConfig : ItemConfig<AmmoType>
{
}
