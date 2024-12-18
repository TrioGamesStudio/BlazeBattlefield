using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponBackpackUI : BindingWeaponUI
{
    [SerializeField] private TextMeshProUGUI gunName;
    [SerializeField] private TextMeshProUGUI ammoTypeName;
    [Header("Gun Stats")]
    [SerializeField] private GameObject gunStats;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI recoilText;
    [SerializeField] private TextMeshProUGUI fireRateText;

    [Header("Stat Colors")]
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private Color recoilColor = Color.green;
    [SerializeField] private Color fireRateColor = Color.yellow;

    protected override void UpdateNewGunInformation()
    {
        base.UpdateNewGunInformation();

        //Debug.Log("On Update Gun Infor");
        gunName.text = weaponSlotHandler.Config.displayName;
        ammoTypeName.text = weaponSlotHandler.Config.ammoUsingType.displayName;
        RefreshGunStats();
    }

    private void RefreshGunStats()
    {
        gunStats.gameObject.SetActive(true);

        float recoilValue = 1 / weaponSlotHandler.Config.cooldownTime;
        recoilValue = (float)Math.Round(recoilValue, 2); 
        damageText.text = $"Damage: <color=#{ColorUtility.ToHtmlStringRGB(damageColor)}>{weaponSlotHandler.Config.damagePerHit}</color>";
        recoilText.text = $"Recoil: <color=#{ColorUtility.ToHtmlStringRGB(recoilColor)}>{recoilValue}</color>";
        fireRateText.text = $"Fire Rate: <color=#{ColorUtility.ToHtmlStringRGB(fireRateColor)}>{weaponSlotHandler.Config.cooldownTime}</color>/s";
    }

    protected override void ResetToDefaultState()
    {
        base.ResetToDefaultState();
        gunName.text = "";
        ammoTypeName.text = "";
        gunStats.gameObject.SetActive(false);
    }
}
