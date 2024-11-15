using UnityEngine;

public class AutoLinkWeaponUISupport : MonoBehaviour
{
    [SerializeField] private BindingWeaponUI[] BindingWeaponUIs;
    private void Start()
    {
        foreach(var item in BindingWeaponUIs)
        {
            int index = (int)item.WeaponIndex;
            item.BindWeaponSlot(WeaponManager.instance.WeaponSlotHandlers[index]);
        }
    }
}
