using UnityEngine;
using System;

public class Player_SniperScopeZoom : MonoBehaviour
{
    public event EventHandler OnRifeUp;
    public event EventHandler OnRifeDown;

    bool isScopeZoom = false;
    [SerializeField] Camera localCam;
    [SerializeField] Animator anim;
    [SerializeField] WeaponHandler weaponHandler;

    private void Start() {
        weaponHandler = GetComponent<WeaponHandler>();
        weaponHandler.OnRifeDown += WeaponHandler_OnRifleDown;
        weaponHandler.OnRifeUp += WeaponHandler_OnRifleUp;

    }

    private void WeaponHandler_OnRifleDown(object sender, EventArgs e)
    {
        anim.SetBool("isScopeZoom", false);
        localCam.nearClipPlane = 0.3f;
        localCam.fieldOfView = 60;
    }

    private void WeaponHandler_OnRifleUp(object sender, EventArgs e)
    {
        anim.SetBool("isScopeZoom", true);
        localCam.nearClipPlane = 0.2f;
        localCam.fieldOfView = 6;
        Debug.Log($"_____ co goi up");
    }

}
