using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using System.Threading.Tasks;
using UnityEngine.Serialization;

public class PlayerController : NetworkBehaviour
{
    [Networked] public string playerName { get; set; }
    [Networked] public int playerIndentify { get; set; }
    [Header("Prefabs")]
    [SerializeField] private Soldier SoldierPrefab;
    [SerializeField] private Camera CameraPrefab;
    [Header("Scriptable Object")]
    [SerializeField] private PlayerData playerData;
    [SerializeField] private GameObject teamMemberPanel;
    [Networked] public NetworkBool IsReady { get; set; }
    [Networked] public NetworkBool IsRoomOwner { get; set; }
    [Networked] public NetworkString<_128> RoomID { get; set; }
    public static PlayerController LocalPlayer;
    private InputPlayerHandler InputPlayerHandler;
    private Matchmaking matchmaking;

    private void Awake()
    {
        InputPlayerHandler = GetComponent<InputPlayerHandler>();
    }

    public override void Spawned()
    {
        base.Spawned();
        // call in every client object
        //transform.name = "Player_" + playerIndentify;
        //// Just add input for Local Player
        //if (HasStateAuthority)
        //{
        //    InputReader.Instance.Enable();
        //    // inputPlayerMovement.EnableInput();
        //    InputPlayerHandler.enabled = true;
        //    LocalPlayer = this;
        //}

        IsReady = false;
        //IsRoomOwner = false;
        matchmaking = FindObjectOfType<Matchmaking>();
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

    public void TurnOnTeamMemberPanel()
    {
        teamMemberPanel.SetActive(true);
    }

    public void ToggleReady()
    {
        //if (Object.HasInputAuthority)
        {
            RPC_SetReady(!IsReady);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetReady(NetworkBool isReady)
    {
        IsReady = isReady;
        matchmaking.UpdatePlayButtonInteractability();
    }

    public void SetAsRoomOwner()
    {
        if (Object.HasStateAuthority)
        {
            IsRoomOwner = true;
            //PlayerPrefs.SetInt("IsRoomOwner", 1);
        }
    }

    public void SetAsRoomMember()
    {
        if (Object.HasStateAuthority)
        {
            IsRoomOwner = false;
            //PlayerPrefs.SetInt("IsRoomOwner", 0);
        }
    }
}
