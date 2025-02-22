using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class PlayerRoomController : NetworkBehaviour
{
    [Networked] public NetworkBool IsReady { get; set; }
    [Networked] public NetworkBool IsRoomOwner { get; set; }
    [Networked] public NetworkBool IsAutoMatch { get; set; }
    [Networked] public NetworkString<_128> RoomID { get; set; }
    [Networked] public NetworkString<_128> TeamID { get; set; }
    [Networked] public NetworkBool IsAlive { get; set; }
    [SerializeField] private GameObject teamMemberPanel;
    public static PlayerRoomController LocalPlayer;
    private Matchmaking matchmaking;
    [Networked] public PlayerRef ThisPlayerRef { get; set; }
    string localRoomId;
    public bool isLocalPlayer = false;

    public GameObject miniMapTeamMateImage;

    private const int POINT_PER_KILL = 10;

    //bool isCursorShowed = false;
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
        //thisPlayerRef = Runner.LocalPlayer;
        IsReady = false;
        IsAlive = true;
        //IsAutoMatch = false;
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

            FindObjectOfType<PressToTalk>().RPC_RequestTeamID(roomID);
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

    public void SetAutoMatch(bool isAutoMatch)
    {
        if (Object.HasStateAuthority)
        {
            //IsAutoMatch = isAutoMatch;
            //Debug.Log("SET AUTO MATCH");
            //PlayerPrefs.SetString("RoomID", roomID); // Save player room
            if (isAutoMatch == true)
                PlayerPrefs.SetInt("IsAutoMatch", 1);
            else
                PlayerPrefs.SetInt("IsAutoMatch", 0);
            RPC_SetAutoMatch(isAutoMatch);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetAutoMatch(bool isAutoMatch)
    {
        IsAutoMatch = isAutoMatch;
        Debug.Log("SET AUTO MATCH");
    }

    public void SetTeamID(string teamID)
    {
        //if (Object.HasStateAuthority)
        {
            RPC_SetTeamID(teamID);

        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_SetTeamID(string teamID)
    {
        TeamID = teamID;
        
    }

    public void SetTeamMateTag()
    {
        gameObject.tag = "TeamMate";
    }

    public void SetPlayerRef(PlayerRef player)
    {
        ThisPlayerRef = player;
        Debug.Log("///SETUP PLAYERREF: " + ThisPlayerRef);
    }

    public void SetLocalPlayer()
    {
        isLocalPlayer = true;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ShowWin()
    {
        ShowCursor();
        Debug.Log("===WIN ROIIIIII");
        int xpGained;
        int coinAdded = GetCoint(1, PlayerStats.Instance.TotalKill);
        if (matchmaking.currentMode == Matchmaking.Mode.Duo)
        {
            FindObjectOfType<WorldUI>().ShowHideWinUITeam();
            // set winteam variable
            DataSaver.Instance.dataToSave.winTeam += 1;
            xpGained = 50; // XP for duo win
        }
        else
        {
            FindObjectOfType<WorldUI>().ShowHideWinUI();
            // save achivement winSolo
            DataSaver.Instance.dataToSave.winSolo += 1;
            xpGained = 50; // XP for duo win
        }

        // Update XP and rank
        var playerData = DataSaver.Instance.dataToSave;
        playerData.experience += xpGained;

        int currentRank = playerData.rank;
        int nextThreshold = RankSystem.GetNextThreshold(currentRank);

        while (playerData.experience >= nextThreshold)
        {
            playerData.rank++;
            Debug.Log($"Rank Up! New Rank: {RankSystem.GetRankName(playerData.rank)}");
            //FindObjectOfType<ShowPlayerInfo>().SetLevelUp();
            nextThreshold = RankSystem.GetNextThreshold(playerData.rank);
        }

        // Update coin
        playerData.coins += coinAdded;

        // save to firebase datatosave
        DataSaver.Instance.SaveData();
        GetComponent<NetworkPlayer>().localUI.SetActive(false);
        PlayerStats.Instance.MarkEndGame();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ShowLose(int rank)
    {
        ShowCursor();
        Debug.Log("===No teammate remain -> Defeat " + "Top " + rank);
        if (matchmaking.currentMode == Matchmaking.Mode.Duo)
        {
            FindObjectOfType<WorldUI>().HideEliminateUI();
            FindObjectOfType<WorldUI>().ShowHideUIDefeatTeam(rank);
        }
        else
        {
            FindObjectOfType<WorldUI>().ShowHideUI(rank);
        }

        var playerData = DataSaver.Instance.dataToSave;

        // Update coin
        playerData.coins += GetCoint(rank, PlayerStats.Instance.TotalKill);

        // save to firebase datatosave
        DataSaver.Instance.SaveData();
    }
    public int GetCoint(int rank, int kill)
    {
        int coinAdded = 10;
        if (rank == 1)
        {
            coinAdded = 50;
        }
        else if (rank == 2)
        {
            coinAdded = 30;
        }
        else if (rank == 3)
        {
            coinAdded = 20;
        }
        coinAdded += kill * POINT_PER_KILL;
        return coinAdded;
    }
    // on off cursor
    /* void ToggleCursor() {
        isCursorShowed = !isCursorShowed;
        if(isCursorShowed) ShowCursor();
        else HideCursor();
    } */

    void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    /* void HideCursor() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    } */

    public void UpdateMap(string map)
    {
        //if (Object.HasInputAuthority)
        {
            RPC_UpdateMap(map);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_UpdateMap(string map)
    {
        //if (Object.HasStateAuthority)
        {
            matchmaking.UpdateMapProperty(map);
        }
    }

    // Broadcast the remaining time to all clients
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_BroadcastTimer(float remainingTime)
    {
        // Update the UI for clients
        FindObjectOfType<UIController>().SetWaitingTime(remainingTime.ToString("F0"));

        if (remainingTime <= 0)
            FindObjectOfType<UIController>().TurnOffWaitingTime();
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        Debug.Log("/// Despawn called for runner: " + runner.name);

        // Ensure that authority changes only if a bot is present
        if (matchmaking.HasBot())
        {
            Debug.Log("/// Bot detected, checking for authority transfer...");
            BotAINetwork[] botAIsNetwork = FindObjectsOfType<BotAINetwork>();

            // Select the next player to take authority
            foreach (var player in runner.ActivePlayers)
            {
                if (player == Runner.LocalPlayer) // Exclude the current local player
                {
                    Debug.Log("/// Transferring StateAuthority to player: " + player.PlayerId);
                    foreach (var botAI in botAIsNetwork)
                    {
                        botAI.RequestAuthority();
                    }
                    break; // Ensure only one player is selected
                }
            }          
        }
    }
}
