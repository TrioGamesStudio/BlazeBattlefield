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
        WeaponManager.OnInitData += BindSlotHandle;
    }
    private void OnDestroy()
    {
        WeaponManager.OnInitData -= BindSlotHandle;
    }

    public void BindSlotHandle(WeaponSlotHandler[] weaponSlotHandlers)
    {
        Debug.Log("BindSlotHandle", gameObject);

        for (int i = 0; i < weaponSlotUIs.Length; i++)
        {
            weaponSlotUIs[i].BindWeaponSlotHandler(weaponSlotHandlers[i]);
        }
    }
}
