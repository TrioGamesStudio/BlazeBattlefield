using UnityEngine;
using Fusion;

public class PlayerNotify : NetworkBehaviour
{
    HPHandler HPHandler;
    private bool isShowResultTable = false;
    private void Awake()
    {
        HPHandler = GetComponent<HPHandler>();
        HPHandler.OnPlayerDeathLocal += Test;
    }

    private void OnDestroy()
    {
        HPHandler.OnPlayerDeathLocal -= Test;
    }

    public void Test()
    {
        if (!isShowResultTable && HPHandler.Networked_HP <= 0)
        {
            isShowResultTable = true;
            if (Matchmaking.Instance.currentMode == Matchmaking.Mode.Solo)
            {
                //PlayerRef player = GetComponent<PlayerRoomController>().ThisPlayerRef;
                //Debug.Log("====Player ref " + player);
                //RPC_ShowResult(Matchmaking.Instance.alivePlayer);    
                //Matchmaking.Instance.CheckWin(player);
                PlayerRoomController playerRoomController = GetComponent<PlayerRoomController>();
                EliminatePlayer(playerRoomController.TeamID.ToString(), playerRoomController);
                HideLocalPlayerUI();
                ShowResultDuo();
            }
            else
            {
                PlayerRoomController playerRoomController = GetComponent<PlayerRoomController>();
                //FindObjectOfType<GameHandler>().Eliminate(playerRoomController.RoomID.ToString(), playerRoomController);
                EliminatePlayer(playerRoomController.TeamID.ToString(), playerRoomController);
                HideLocalPlayerUI();
                ShowResultDuo();
            }
        }

        
    }
    public void EliminatePlayer(string teamID, PlayerRoomController playerRoomController)
    {
        RPC_EliminatePlayer(teamID, playerRoomController);
    }

    public void ShowResultDuo()
    {
        RPC_ShowResultDuo();
    }
    public void HideLocalPlayerUI()
    {
        RPC_HideLocalPlayerUI();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_EliminatePlayer(string teamID, PlayerRoomController playerRoomController)
    {
        FindObjectOfType<GameHandler>().Eliminate(teamID, playerRoomController);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_ShowResultDuo()
    {
        GetComponent<PlayerRoomController>().IsAlive = false;
        Debug.Log(":::Player shut down");
        StartCoroutine(FindObjectOfType<GameHandler>().CheckLose(GetComponent<PlayerRoomController>().TeamID.ToString()));
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_HideLocalPlayerUI()
    {
        NetworkPlayer.Local.localUI.SetActive(false);
    }
}