using Fusion;
using System;
using UnityEngine;

public class Soldier : NetworkBehaviour
{
    public SoldierMovement movement;
    public Transform cameraPositionTransform;
    private void Awake()
    {
        movement = GetComponent<SoldierMovement>();
    }

}