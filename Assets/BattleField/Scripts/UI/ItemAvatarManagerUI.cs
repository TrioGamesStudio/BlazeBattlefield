using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAvatarManagerUI : MonoBehaviour
{
    public ItemAvatarUI prefab;
    public GameObject cointaner;
    public List<ScriptableObject> itemConfigs = new();

    private void Init()
    {
        foreach (var item in itemConfigs)
        {
            var itemAvatar = Instantiate(prefab, cointaner.transform);

            if(item is ItemConfig<Enum> itemConfig)
            {
                itemAvatar.displayName.text = itemConfig.displayName;
                if (item is AmmoItemConfig ammoConfig)
                {
                    itemAvatar.information.text = "None";
                }
                else if (item is GunItemConfig gunConfig)
                {
                    float recoilValue = 1 / gunConfig.cooldownTime;
                    recoilValue = (float)Math.Round(recoilValue, 2);
                    itemAvatar.information.text = $"Damage: {gunConfig.damagePerHit} \nRecoil: {recoilValue}\n Fire Rate: {gunConfig.cooldownTime}/s\nAmmo: {gunConfig.ammoUsingType.displayName}"; ;
                }
                else if (item is HealthItemConfig healthConfig)
                {
                    itemAvatar.information.text = $"Healing Amount: {healthConfig.healthAmount}\nUse Time: {healthConfig.usingTime}";
                }
            }
        }
    }
}
