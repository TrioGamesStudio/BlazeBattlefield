using Fusion.Sockets;
using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : PlayerSpawnerBase<PlayerController>, IPlayerJoined, INetworkRunnerCallbacks
{
    private NetworkRunner runner;
    private void Awake()
    {
        runner = GetComponent<NetworkRunner>();
        runner.AddCallbacks(this);
    }
    public void PlayerJoined(PlayerRef player)
    {
        if (player == runner.LocalPlayer)
        {
            var playerObj = runner.Spawn(PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity, player, (Runner, obj) =>
            {
                var _player = obj.GetComponent<PlayerController>();
                _player.playerIndentify = player.AsIndex;
                _player.Setup();

            });
            playerObj.LinkSoldierToPlayer();

        }

        Debug.Log(player.PlayerId);
    }
 

    #region FusionCallback
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        if (PlayerController.LocalPlayer == null) return;
        runner.Despawn(PlayerController.LocalPlayer.Object);
    }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {

        Debug.LogWarning("OnSessionListUpdated");
        //EventManager.OnReceiveRoomInformation?.Invoke(sessionList.ToRoomInfor());
    }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    #endregion FusionCallback


}