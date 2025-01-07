using System;
using UnityEngine;

public interface IRunTimeItem
{
    public bool isDisplayedUI { get; set; }
    public Action<IRunTimeItem> OnRemoveItemUI { get; set; }
    public Sprite GetIcon();
    string DisplayName();
    string UniqueID();
    int Quantity();
    void Collect();
    void CollectAI(ActiveWeaponAI activeWeaponAI);
    void DestroyItem();
    void DisableOutline();
}
