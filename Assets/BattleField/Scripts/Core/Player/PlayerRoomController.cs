using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerRoomController : NetworkBehaviour
{
    [Networked] public NetworkBool IsReady { get; set; }
    [Networked] public NetworkBool IsRoomOwner { get; set; }
    [Networked] public NetworkString<_128> RoomID { get; set; }
    [SerializeField] private GameObject teamMemberPanel;
    public static PlayerRoomController LocalPlayer;
    private Matchmaking matchmaking;
    string localRoomId;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Spawned()
    {
        base.Spawned();
        IsReady = false;
        //IsRoomOwner = false;
        matchmaking = FindObjectOfType<Matchmaking>();
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

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SetAsRoomOwner()
    {
        if (Object.HasStateAuthority)
        {
            Debug.Log("Set room owner");
            IsRoomOwner = true;
            PlayerPrefs.SetInt("IsRoomOwner", 1);
        }
    }

    public void SetAsRoomMember()
    {
        if (Object.HasStateAuthority)
        {
            IsRoomOwner = false;
            PlayerPrefs.SetInt("IsRoomOwner", 0);
        }
    }

    public void SetRoomID(string roomID)
    {
        if (Object.HasStateAuthority)
        {
            RoomID = roomID;
            localRoomId = roomID;
            Debug.Log("SET ROOM ID");
            PlayerPrefs.SetString("RoomID", roomID); // Save player room
        }
    }

    #region Team 

    public void SetBattleRoom()
    {
        //if (Object.HasStateAuthority)
        {
            RPC_SetBattleRoom();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SetBattleRoom()
    {
        //BattleRoomName = roomName;
        //Debug.Log(BattleRoomName.ToString());
        //IsReady = isReady;
        Debug.Log("Join team battle room call from remote to local");
        matchmaking.RPC_TransitionAllToBattleRoom();
    }


    #endregion
}
