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

    protected override void UpdateGunInfor()
    {
        base.UpdateGunInfor();

        //Debug.Log("On Update Gun Infor");
        gunName.text = weaponSlotHandler.Config.displayName;
        ammoTypeName.text = weaponSlotHandler.Config.ammoUsingType.displayName;


    }

    //protected override void ResetUIState()
    //{
    //    base.ResetUIState();
    //    gunName.text = "";
    //    ammoTypeName.text = "";

    //}
}
