using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearActive : NetworkBehaviour
{
    [SerializeField] private GameObject[] armors;
    [SerializeField] private GameObject[] helmets;
    [SerializeField] private GameObject[] backpacks;
    public static int TURN_OFF_INDEX = -1;
    private void Awake()
    {
        TurnOffAll(armors);
        TurnOffAll(helmets);
        TurnOffAll(backpacks);
    }

    public override void Spawned()
    {
        base.Spawned();
        if (HasStateAuthority)
        {
            GearManager.instance.gearActive = this;
        }
    }


    [Rpc(RpcSources.StateAuthority,RpcTargets.All)]
    public void ShowArmorRPC(int level)
    {
        ActivateGear(level,armors);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void ShowHelmetRPC(int level)
    {
        ActivateGear(level, helmets);
    }


    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void ShowBackpackRPC(int level)
    {
        ActivateGear(level, backpacks);
    }
    private void ActivateGear(int level, GameObject[] array)
    {
        TurnOffAll(array);
        if (level == TURN_OFF_INDEX)
        {
            return;
        }
        array[level].gameObject.SetActive(true);
    }


    private void TurnOffAll(GameObject[] arrayObject)
    {
        foreach(var item in arrayObject)
        {
            item.gameObject.SetActive(false);
        }
    }
}
