using Fusion;
using NaughtyAttributes;
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
    public static PlayerMessageManager instance;
    [SerializeField] MessageDisplayUI localMessageDisplayUI;
    [SerializeField] MessageDisplayUI globalMessageDisplayUI;
    [SerializeField] IconMessageSO IconMessageSO;
    [SerializeField] private bool isShowEnterExitLog = true;
    //public void MessageLogType(string fullText, MessageLogType type, bool isLocal)
    //{
    //    localMessageDisplayUI.CreateFullMessage(fullText, IconMessageSO.GetIcon(type));

    //}
    public void FallOffLog(string playerName)
    {
        localMessageDisplayUI.CreateFullMessage(playerName, IconMessageSO.GetIcon(MessageLogType.FallOff));
    }

    public string fullTextTest;
    public MessageLogType type;
    [EditorButton]
    public void SendLog()
    {
        //MessageLogType(fullTextTest, type, true);
    }

    public void SendKillLog(string v1, string v2)
    {
        localMessageDisplayUI.CreateLogMessage(v1, v2, IconMessageSO.GetIcon(MessageLogType.KillLog));
    }

    public void ExitLog(string v)
    {
        if (isShowEnterExitLog == false) return;
        localMessageDisplayUI.CreateFullMessage(v, IconMessageSO.GetIcon(MessageLogType.LeaveLog));
    }

    public void EnterLog(string v)
    {
        if (isShowEnterExitLog == false) return;
        localMessageDisplayUI.CreateFullMessage(v, IconMessageSO.GetIcon(MessageLogType.JoinLog));
    }
}
