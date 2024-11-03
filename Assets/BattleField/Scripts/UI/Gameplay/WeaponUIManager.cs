using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUIManager : MonoBehaviour
{
    public static WeaponUIManager instance;
    public WeaponSlotUI[] weaponSlotUIs;
    private void Awake()
    {
        instance = this;
        WeaponManager.instance.OnInitData += BindSlotHandle;
    }
    private void OnDestroy()
    {
        WeaponManager.instance.OnInitData -= BindSlotHandle;
    }

    public void BindSlotHandle(WeaponSlotHandler[] weaponSlotHandlers)
    {
        for (int i = 0; i < weaponSlotUIs.Length; i++)
        {
            weaponSlotUIs[i].BindWeaponSlotHandler(weaponSlotHandlers[i]);
        }
    }
}
