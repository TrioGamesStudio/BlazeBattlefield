using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Ammo", menuName = "Config/Ammo")]
public class AmmoItemConfig : ItemConfig<AmmoType>
{
    public int totalAmmo;
    [Button]
    private void LoadDefaultSettings()
    {
        switch (SubItemType)
        {
            case AmmoType.None:
                break;
            case AmmoType.Ammo556:
                ItemConfigFactory.CreateAmmoConfig(this, AmmoType.Ammo556, "5.56mm Ammo", 30);
                break;
            case AmmoType.Ammo762:
                ItemConfigFactory.CreateAmmoConfig(this,AmmoType.Ammo762, "7.62mm Ammo", 30);
                break;
            case AmmoType.Ammo9mm:
                ItemConfigFactory.CreateAmmoConfig(this,AmmoType.Ammo9mm, "0.9mm Ammo", 30);
                break;
            case AmmoType.Ammo12Gauge:
                ItemConfigFactory.CreateAmmoConfig(this,AmmoType.Ammo12Gauge, "12 Gauge Ammo", 30);
                break;
            default:
                break;
        }

    }
}
