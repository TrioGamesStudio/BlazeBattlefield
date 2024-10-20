using Fusion;
using System;
using UnityEngine;

public class Soldier : NetworkBehaviour
{
    public SoldierMovement movement;
    public Transform cameraPositionTransform;
    public Test CollectCube;
    private void Awake()
    {
        movement = GetComponent<SoldierMovement>();
        CollectCube.gameObject.SetActive(false);
    }

    public void SetupCollectCube()
    {
        CollectCube.gameObject.SetActive(true);
    }
}