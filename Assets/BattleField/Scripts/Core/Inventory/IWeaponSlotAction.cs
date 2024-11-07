using System;

public interface IWeaponSlotAction
{
    Action ShowWeaponAction { get; set; }
    Action HideWeaponAction { get; set; }
    Action EquipWeaponAction { get; set; }
    Action DropWeaponAction { get; set; }
}
