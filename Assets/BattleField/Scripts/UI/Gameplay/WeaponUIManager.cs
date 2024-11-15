using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUIManager : MonoBehaviour
{
    public static WeaponUIManager instance;
    [SerializeField] private WeaponSlotUI[] weaponSlotUIArray;
    private void Awake()
    {
        instance = this;
        weaponSlotUIArray = GetComponentsInChildren<WeaponSlotUI>();
    }
    public void Hightligh(int index)
    {
        Debug.Log("Hightlight in :" + index);
        foreach (var item in weaponSlotUIArray)
        {
            item.RemoveHighlight();
        }
        if (index == -1) return;

        weaponSlotUIArray[index].ApplyHighlight();

    }
}
