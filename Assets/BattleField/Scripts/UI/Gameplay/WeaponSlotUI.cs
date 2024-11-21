using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;
public class WeaponSlotUI : BindingWeaponUI
{
    [SerializeField] protected Image hightlightFrameImg;

    public void ApplyHighlight()
    {
        hightlightFrameImg.gameObject.SetActive(true);
    }
    public void RemoveHighlight()
    {
        hightlightFrameImg.gameObject.SetActive(false);
    }

    protected override void UpdateIcon()
    {
        IconImage.gameObject.SetActive(true);
        IconImage.sprite = weaponSlotHandler.Config.IconActualGun;
    }
}
