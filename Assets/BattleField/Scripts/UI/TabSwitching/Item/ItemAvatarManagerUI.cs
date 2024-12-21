using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemAvatarManagerUI : MonoBehaviour
{
    public ItemAvatarUI prefab;
    public GameObject cointaner;
    public List<ScriptableObject> itemConfigs = new();
    public List<ICustomInformation> customInformations = new();

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
        foreach (var item in itemConfigs)
        {
            var itemAvatar = Instantiate(prefab, cointaner.transform);
            TryModify(item, itemAvatar);
        }
    }

    private void TryModify(ScriptableObject item, ItemAvatarUI itemAvatar)
    {
        bool canModify = false;
        foreach (var customInformation in customInformations)
        {
            if (customInformation.Modify(itemAvatar, item))
            {
                canModify = true;
                break;
            }
        }
        if(canModify == false)
        {
            itemAvatar.information.text = "None";
        }
    }

    public interface ICustomInformation
    {
        bool Modify(ItemAvatarUI itemAvatarUI, ScriptableObject scriptableObject);
    }
    public class GunInformation : ICustomInformation
    {
        public bool Modify(ItemAvatarUI itemAvatarUI, ScriptableObject scriptableObject)
        {
            if (scriptableObject is GunItemConfig gunConfig)
            {
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
        public bool Modify(ItemAvatarUI itemAvatarUI, ScriptableObject scriptableObject)
        {
            if (scriptableObject is HealthItemConfig healthConfig)
            {
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
        public bool Modify(ItemAvatarUI itemAvatarUI, ScriptableObject scriptableObject)
        {
            if (scriptableObject is AmmoItemConfig ammoConfig)
            {
                itemAvatarUI.displayName.text = ammoConfig.displayName;
                itemAvatarUI.icon.sprite = ammoConfig.Icon;
                itemAvatarUI.information.text = "<b>None<b>";
                return true;
            }
            return false;
        }
    }
}
