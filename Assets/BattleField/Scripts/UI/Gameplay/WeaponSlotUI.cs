using DG.Tweening;
using NaughtyAttributes;
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
    [SerializeField] protected float scaleUpTime = 0.1f;
    [SerializeField] protected float scaleDownTime = 0.1f;
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

    protected override void UpdateCurrentAmmo(int currentAmmo)
    {
        base.UpdateCurrentAmmo(currentAmmo);

        currentGunAmmoText.transform.DOKill();
        currentGunAmmoText.transform.DOScale(Vector3.one * 1.2f, scaleUpTime).OnComplete(() =>
        {
            currentGunAmmoText.transform.DOScale(Vector3.one, scaleDownTime);
        });
    }
}
