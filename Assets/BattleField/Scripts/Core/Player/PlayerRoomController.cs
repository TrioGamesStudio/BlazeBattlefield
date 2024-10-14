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
    public static PlayerController LocalPlayer;
    private Matchmaking matchmaking;
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
