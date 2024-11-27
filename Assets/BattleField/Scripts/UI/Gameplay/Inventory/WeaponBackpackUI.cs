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

    protected override void UpdateNewGunInformation()
    {
        base.UpdateNewGunInformation();

        //Debug.Log("On Update Gun Infor");
        gunName.text = weaponSlotHandler.Config.displayName;
        ammoTypeName.text = weaponSlotHandler.Config.ammoUsingType.displayName;
    }

    protected override void ResetToDefaultState()
    {
        base.ResetToDefaultState();
        gunName.text = "";
        ammoTypeName.text = "";
    }
}
