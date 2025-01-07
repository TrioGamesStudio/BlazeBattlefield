using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Health", menuName = "Config/Health")]
public class HealthItemConfig : ItemConfig<HealingItemType>
{
    public byte healthAmount;
    public float usingTime;

    public override void ShowDebug()
    {
        base.ShowDebug();
        Debug.Log($"Additional: HealthAmount {healthAmount} usingTime {usingTime}");
    }

    [Button]
    private void LoadDefaultSettings()
    {
        switch (SubItemType)
        {
            case HealingItemType.None:
                break;
            case HealingItemType.Bandage:
                ItemConfigFactory.CreateHealthItem(this,HealingItemType.Bandage, "Bandage", 10, 15, .8f);
                break;
            case HealingItemType.FirstAidKit:
                ItemConfigFactory.CreateHealthItem(this, HealingItemType.FirstAidKit, "First Aid Kit", 5, 70, 1.5f);
                break;
            case HealingItemType.Medkit:
                ItemConfigFactory.CreateHealthItem(this, HealingItemType.Medkit, "Med Kit", 5, 100, 2f);
                break;
            default:
                break;
        }

    }
}
