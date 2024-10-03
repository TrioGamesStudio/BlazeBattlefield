using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using System.Threading.Tasks;
public class PlayerController : NetworkBehaviour
{
    [Networked] public string playerName { get; set; }
    [Networked] public int playerIndentify { get; set; }
    [Header("Prefabs")]
    [SerializeField] private Soldier SoldierPrefab;
    [SerializeField] private Camera CameraPrefab;
    [Header("Scriptable Object")]
    [SerializeField] private PlayerData playerData;
    [SerializeField] private InputReader inputReader;
    

    public static PlayerController LocalPlayer;

   
    private InputPlayerHandler InputPlayerHandler;
    private void Awake()
    {
        InputPlayerHandler = GetComponent<InputPlayerHandler>();
    }
    public override void Spawned()
    {
        base.Spawned();
        // call in every client object
        transform.name = "Player_" + playerIndentify;
        // Just add input for Local Player
        if (HasStateAuthority)
        {
            inputReader.EnableInput();
            InputPlayerHandler.SetupInput(inputReader);
            InputPlayerHandler.enabled = true;
            LocalPlayer = this;
        }
     
    }

    public void Setup()
    {
        if (HasStateAuthority)
        {
            playerData.CreateRandomPlayerName();
            playerName = playerData.PlayerName;
        }
    }


    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void LinkSoilderToPlayerRPC()
    {
        LocalPlayer.LinkSoldierToPlayer();
    }

    public void LinkSoldierToPlayer()
    {
        if (HasStateAuthority)
        {
            Camera.main.gameObject.SetActive(false);
            var camera = Instantiate(CameraPrefab).GetComponent<LocalCameraBase>();
            var soldier = Runner.Spawn(SoldierPrefab, Vector3.zero,Quaternion.identity);
            camera.SetTarget(soldier.cameraPositionTransform);    
            // Link input to soldier and cameraLook
            InputPlayerHandler.SwitchMovementController(soldier.movement);
            InputPlayerHandler.SwitchCameraController(camera);
        }
    }
}
