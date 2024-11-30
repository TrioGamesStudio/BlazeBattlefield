using Fusion;
using System;
using System.Collections;
using UnityEngine;
using static Fusion.Sockets.NetBitBuffer;

public class PlayerMessageManager : NetworkBehaviour
{
    public static PlayerMessageManager instance;

    public enum MessageLogType
    {
        KillLog = 0,
        JoinLog = 10,
        LeaveLog = 15
    }
    [SerializeField] private LocalMessageUI localMessageUIPrefab;
    [SerializeField] private GameObject globalMessageHolder;
    [SerializeField] private GameObject localMessageHolder;
    private LocalMessageUI CreateRawMessage()
    {
        return Instantiate(localMessageUIPrefab, globalMessageHolder.transform);
    }

    public void SendKillLog(string killedName,string killerName )
    {
        var message = CreateRawMessage();

        message.PassMessageData(killerName, killedName);

        message.gameObject.SetActive(true);

        Destroy(message.gameObject, 3f);
    }
    int index = 0;
    [EditorButton]
    private void Test()
    {
        index++;
        SendKillLog("some one"+ index.ToString(), "anybody");
    }

}
