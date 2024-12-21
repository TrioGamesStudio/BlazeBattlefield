using Fusion;
using System;
using System.Collections;
using UnityEngine;
public enum MessageLogType
{
    None,
    KillLog = 5,
    JoinLog = 10,
    LeaveLog = 15,
    FallOff = 20,
}
public class PlayerMessageManager : NetworkBehaviour
{
    [SerializeField] MessageDisplayUI localMessageDisplayUI;
    [SerializeField] IconMessageSO IconMessageSO;
    [SerializeField] private bool isShowEnterExitLog = true;


    public void Fall(string playerName)
    {
        FallOffLogRPC(playerName);
    }

    [Rpc(RpcSources.StateAuthority,RpcTargets.All)]
    public void FallOffLogRPC(string playerName)
    {
        Check();
        localMessageDisplayUI.CreateFullMessage(playerName, IconMessageSO.GetIcon(MessageLogType.FallOff));
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void SendKillLogRPC(string v1, string v2)
    {
        Check();
        localMessageDisplayUI.CreateLogMessage(v1, v2, IconMessageSO.GetIcon(MessageLogType.KillLog));
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void ExitLogRPC(string v)
    {
        Check();
        if (isShowEnterExitLog == false) return;
        localMessageDisplayUI.CreateFullMessage(v, IconMessageSO.GetIcon(MessageLogType.LeaveLog));
    }
     [Rpc(RpcSources.StateAuthority,RpcTargets.All)]
    public void EnterLogRPC(string v)
    {
        Check();
        if (isShowEnterExitLog == false) return;
        localMessageDisplayUI.CreateFullMessage(v, IconMessageSO.GetIcon(MessageLogType.JoinLog));
    }
    
    private void Check()
    {
        if(localMessageDisplayUI == null)
        {
            localMessageDisplayUI = NetworkPlayer.Local.LocalCameraHandler.GetComponentInChildren<MessageDisplayUI>();
        } 
    }
}
