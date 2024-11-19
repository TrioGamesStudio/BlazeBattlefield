using Fusion;

public class MessagesNotify : NetworkBehaviour
{
    NetworkInGameMessages networkInGameMessages;
    HPHandler HPHandler;
    private void Awake()
    {
        networkInGameMessages = GetComponent<NetworkInGameMessages>();
        HPHandler = GetComponent<HPHandler>();

        HPHandler.OnKillPlayerMessages += OnShowKillMessages;
    }
    private void OnDestroy()
    {
        HPHandler.OnKillPlayerMessages -= OnShowKillMessages;
    }

    private void OnShowKillMessages()
    {
        networkInGameMessages.SendInGameRPCMessage(HPHandler.Networked_Killer.ToString(),
                $" killed <b>{NetworkPlayer.Local.nickName_Network.ToString()}<b>");
    }
}