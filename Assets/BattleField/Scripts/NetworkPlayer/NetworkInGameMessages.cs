using UnityEngine;
using Fusion;

//todo gameobject = player
//todo HasInputAuthority input local name -> RPC_SetNickName() -> thong bao ten ra man hinh
public class NetworkInGameMessages : NetworkBehaviour
{
    //! neu keo tha doi tuong vao day - se ko in o man hinh killer
    [SerializeField] InGameMessagesUIHandler inGameMessagesUIHandler;

    public void SendInGameRPCMessage(string nickName, string message)   // col 92 NetworkPlayer.cs
    {
        RPC_InMessage($"<b>{nickName}<b>{message}");
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_InMessage(string message, RpcInfo info = default) {
        Debug.Log($"[RPC] InGameMessage {message}");

        // InGameMessagesUIHandler.cs nam trong camera (localCamera.cs)
        // camera object da out khoi player Object khi spwan ra
        // this class (nam trong player object) muon truy cap InGameMessagesUIHandler.cs
        // phai thong qua NetworkPlayer.cs de dung localCamera CHA lay thuoc tinh InGameMessagesUIHandler.cs CON
        if(inGameMessagesUIHandler == null) {
            inGameMessagesUIHandler = NetworkPlayer.Local.LocalCameraHandler
                                    .GetComponentInChildren<InGameMessagesUIHandler>();
            
            //inGameMessagesUIHandler = NetworkPlayer.Local.LocalCameraHandler.InGameMessagesUIHandler;
        }
        if(inGameMessagesUIHandler != null) {
            inGameMessagesUIHandler.OnGameMessageRecieved(message);
        }
        
    }
}