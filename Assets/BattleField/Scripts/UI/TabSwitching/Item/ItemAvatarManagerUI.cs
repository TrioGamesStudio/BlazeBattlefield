using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
[Serializable]
public struct OffsetInformation
{
    public Vector3 offsetPosition;
    public Vector3 offsetRotation;
}
public class WeaponShowcaseSettings : ScriptableObject
{

}
public class ItemAvatarManagerUI : MonoBehaviour
{

    [SerializeField] private ItemAvatarUI prefab;
    [SerializeField] private GameObject cointaner;
    [SerializeField] private List<ScriptableObject> itemConfigs = new();
    [SerializeField] private List<OffsetInformation> itemOffsetConfigs = new();
    [SerializeField] private List<GameObject> ItemPrefabs;
    [SerializeField] private List<ICustomInformation> customInformations = new();
    [SerializeField] private WeaponShowcase weaponShowcase;
    private void Awake()
    {
        InitCustomInformation();
        InitItemUI();
    }

    private void InitCustomInformation()
    {
        customInformations = new()
        {
            new GunInformation(),
            new HealthInformation(),
            new AmmoInformation()
        };
    }

    private void InitItemUI()
    {
        int index = 0;
        foreach (var item in itemConfigs)
        {
            var itemAvatar = Instantiate(prefab, cointaner.transform);
            TryModify(item, itemAvatar);

            itemAvatar.ItemIndex = index;
            itemAvatar.OnClickUI += OnClickUI;
            index++;
        }
    }

    private void OnClickUI(int index)
    {
        Debug.Log("On set prefab and create item in table: " + index);
        weaponShowcase.SetItemPrefab(ItemPrefabs[index]);
        weaponShowcase.SetOffset(itemOffsetConfigs[index].offsetPosition, itemOffsetConfigs[index].offsetRotation);
        weaponShowcase.CreateItem();
    }

    private void TryModify(ScriptableObject item, ItemAvatarUI itemAvatar)
    {
        bool canModify = false;
        GameObject prefab = null;
        foreach (var customInformation in customInformations)
        {
            if (customInformation.Modify(itemAvatar, item, ref prefab))
            {
                canModify = true;

                ItemPrefabs.Add(prefab);
                break;
            }
        }
        if (canModify == false)
        {
            itemAvatar.information.text = "None";
        }
    }

    public interface ICustomInformation
    {
        bool Modify(ItemAvatarUI itemAvatarUI, ScriptableObject scriptableObject, ref GameObject prefab);
    }
    public class GunInformation : ICustomInformation
    {
        public bool Modify(ItemAvatarUI itemAvatarUI, ScriptableObject scriptableObject, ref GameObject prefab)
        {
            if (scriptableObject is GunItemConfig gunConfig)
            {
                prefab = ItemDatabase.instance.GetItemPrefab(gunConfig.ItemType, gunConfig.SubItemType);

                itemAvatarUI.displayName.text = gunConfig.displayName;
                itemAvatarUI.icon.sprite = gunConfig.Icon;
                float recoilValue = 1 / gunConfig.cooldownTime;
                recoilValue = (float)Math.Round(recoilValue, 2);
                itemAvatarUI.information.text = $"<b>Damage<b>: {gunConfig.damagePerHit}" +
                    $"\n<b>Recoil<b>: {recoilValue}\n <b>Fire Rate<b>: {gunConfig.cooldownTime}/s" +
                    $"\n<b>Type<b>: {gunConfig.ammoUsingType.displayName}"; ;
                return true;
            }
            return false;
        }
    }
    public class HealthInformation : ICustomInformation
    {
        public bool Modify(ItemAvatarUI itemAvatarUI, ScriptableObject scriptableObject, ref GameObject prefab)
        {
            if (scriptableObject is HealthItemConfig healthConfig)
            {
                prefab = ItemDatabase.instance.GetItemPrefab(healthConfig.ItemType, healthConfig.SubItemType);


                itemAvatarUI.displayName.text = healthConfig.displayName;
                itemAvatarUI.icon.sprite = healthConfig.Icon;
                itemAvatarUI.information.text = $"<b>Healing Amount<b>: {healthConfig.healthAmount}" +
                    $"\n<b>Use Time<b>: {healthConfig.usingTime}";
                return true;
            }
            return false;
        }
    }
    public class AmmoInformation : ICustomInformation
    {
        public bool Modify(ItemAvatarUI itemAvatarUI, ScriptableObject scriptableObject, ref GameObject prefab)
        {
            if (scriptableObject is AmmoItemConfig ammoConfig)
            {
                prefab = ItemDatabase.instance.GetItemPrefab(ammoConfig.ItemType, ammoConfig.SubItemType);

                itemAvatarUI.displayName.text = ammoConfig.displayName;
                itemAvatarUI.icon.sprite = ammoConfig.Icon;
                itemAvatarUI.information.text = "<b>None<b>";
                return true;
            }
            return false;
        }
    }
}
