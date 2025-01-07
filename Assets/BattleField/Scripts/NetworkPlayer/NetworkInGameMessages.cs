using UnityEngine;
using Fusion;
using Unity.VisualScripting;

//todo gameobject = player
//todo HasInputAuthority input local name -> RPC_SetNickName() -> thong bao ten ra man hinh
public class NetworkInGameMessages : NetworkBehaviour
{
    //! neu keo tha doi tuong vao day - se ko in o man hinh killer
    [SerializeField] InGameMessagesUIHandler inGameMessagesUIHandler;

    public void SendInGameRPCMessage(string nickName, string message)   // col 92 NetworkPlayer.cs
    {
        return;
        RPC_InMessage($"<b>{nickName}<b>{message}");
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_InMessage(string message, RpcInfo info = default) {
        Debug.Log($"[RPC] InGameMessage {message}");

        if(inGameMessagesUIHandler == null) {
            if(NetworkPlayer.Local.LocalCameraHandler == null) return;
            
            inGameMessagesUIHandler = NetworkPlayer.Local.LocalCameraHandler
                                    .GetComponentInChildren<InGameMessagesUIHandler>();
            
            //inGameMessagesUIHandler = NetworkPlayer.Local.LocalCameraHandler.InGameMessagesUIHandler;
        }
        if(inGameMessagesUIHandler != null) {
            inGameMessagesUIHandler.OnGameMessageRecieved(message);
        }
        
    }
}