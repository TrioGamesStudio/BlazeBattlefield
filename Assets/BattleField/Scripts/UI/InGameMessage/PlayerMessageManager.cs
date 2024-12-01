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
    LeaveLog = 15
}
public class PlayerMessageManager : NetworkBehaviour
{
    public static PlayerMessageManager instance;
    [SerializeField] MessageDisplayUI localMessageDisplayUI;
    [SerializeField] MessageDisplayUI globalMessageDisplayUI;
    [SerializeField] IconMessageSO IconMessageSO;
    public void MessageLogType(string fullText, MessageLogType type, bool isLocal)
    {
        if (isLocal)
        {
            localMessageDisplayUI.CreateFullMessage(fullText, IconMessageSO.GetIcon(type));
        }
        else
        {
            globalMessageDisplayUI.CreateFullMessage(fullText, IconMessageSO.GetIcon(type));
        }
    }
    public string fullTextTest;
    public MessageLogType type;
    [EditorButton]
    public void SendLog()
    {
        MessageLogType(fullTextTest, type, true);
    }
}
