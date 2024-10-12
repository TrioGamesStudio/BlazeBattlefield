using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using System.Linq;

public class Matchmaking : Fusion.Behaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkRunner networkRunnerPrefab;
    [SerializeField] private PlayerController playerControllerPrefab;
    [SerializeField] private List<Transform> memberPos = new();
    [SerializeField] private GameObject localPlayer;
    private NetworkRunner networkRunner;
    private const int MAX_PLAYER = 4;
    public Dictionary<PlayerRef, PlayerController> players = new();

    private Vector3 spawnPosition;
    private Mode currentMode = Mode.Solo;
    //private const int TEAM_SIZE = 2;
    enum SceneBuildIndex
    {
        PlayScene = 2,
    }

    enum Mode
    {
        Solo,
        Duo
    }
    
    void Start()
    {
        networkRunner = Instantiate(networkRunnerPrefab);
        networkRunner.AddCallbacks(this);
        JoinLobby();
    }

    
    void Update()
    {

    }

    public void SetSoloMode()
    {
        currentMode = Mode.Solo;
    }

    public void SetDuoMode()
    {
        currentMode = Mode.Duo;
    }

    public async void JoinLobby()
    {
        // Call this to join the session lobby
        var startTask = networkRunner.JoinSessionLobby(SessionLobby.Shared);

        await startTask;

        if (startTask.Result.Ok)
        {
            //sessionListText.text = "Joined lobby";
            Debug.Log("Joined lobby");
        }
        else
        {
            //sessionListText.text = "Fail";
        }

    }

    public async void QuickPlay()
    {
        if (networkRunner == null)
        {
            networkRunner = Instantiate(networkRunnerPrefab);
            networkRunner.AddCallbacks(this);
        }
        var sceneInfo = new NetworkSceneInfo();
        int playSceneIndex = (int)SceneBuildIndex.PlayScene;
        sceneInfo.AddSceneRef(SceneRef.FromIndex(playSceneIndex));
        var result = await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            Scene = sceneInfo,
            PlayerCount = MAX_PLAYER,
        });

        if (result.Ok)
        {
            // all good
            Debug.Log("Match room name: " + networkRunner.SessionInfo.Name);

        }
        else
        {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }
    }

    public async void CreateRoom()
    {
        string teamcode = UnityEngine.Random.Range(100, 999).ToString();
        if (networkRunner == null)
        {
            networkRunner = Instantiate(networkRunnerPrefab);
            networkRunner.AddCallbacks(this);
        }
        var startArguments = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = teamcode,
            PlayerCount = 2,
        };

        //StatusText.text = startArguments.GameMode == GameMode.Single ? "Starting single-player..." : "Connecting...";

        var result = await networkRunner.StartGame(startArguments);

        if (result.Ok)
        {
            //StatusText.text = "";
            //teamcodeText.text = teamcode;
            //createRoomButton.gameObject.SetActive(false);
            //sessionListContent.parent.parent.gameObject.SetActive(false);
            //FindObjectOfType<UIManager>().TurnOffCreateRoomButton();
            Debug.Log("Team room name: " + networkRunner.SessionInfo.Name);
        }
        else
        {
            //StatusText.text = $"Connection Failed: {result.ShutdownReason}";
        }
    }

    public async void LeaveRoom()
    {
        if (networkRunner != null)
        {
            Debug.Log("Leaving room...");

            // Shuts down the runner and leaves the current session
            await networkRunner.Shutdown();

            Debug.Log("You have left the room.");
            currentMode = Mode.Solo;
            UIController.Instance.SwitchMode(true);
            networkRunner = null;
            localPlayer.SetActive(true);

            // Optionally update the UI, e.g., re-enable room creation UI or show session list
            // FindObjectOfType<UIManager>().TurnOnCreateRoomButton();
            // sessionListContent.parent.parent.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Network runner is not initialized or you are not in a session.");
        }
    }

    public async void JoinRoomByName(string roomName)
    {
        currentMode = Mode.Duo;
        var sceneInfo = new NetworkSceneInfo();
        sceneInfo.AddSceneRef(SceneRef.FromIndex(4)); //Share room scene;
        if (networkRunner == null)
        {
            networkRunner = Instantiate(networkRunnerPrefab);
            networkRunner.AddCallbacks(this);
        }
        var result = await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = roomName,
            Scene = sceneInfo, // Assuming you have a separate battle room scene
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
        });

        if (result.Ok)
        {
            // all good
            //createRoomButton.gameObject.SetActive(false);
            //sessionListContent.parent.parent.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }

    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        
        if (currentMode == Mode.Duo)
        {
            localPlayer.SetActive(false);
            //throw new NotImplementedException();
            if (player == runner.LocalPlayer)
            {
                spawnPosition = memberPos[runner.ActivePlayers.Count() - 1].position;
                PlayerController playerObject = runner.Spawn(playerControllerPrefab, spawnPosition, Quaternion.identity, player);
                runner.SetPlayerObject(runner.LocalPlayer, playerObject.Object);
                players[player] = playerObject.GetComponent<PlayerController>();
                players[player].TurnOnTeamMemberPanel();
                Debug.Log("New player joined " + player.ToString());
                Debug.Log("Player count " + runner.ActivePlayers.Count());
                //players[player].SetRoomID(runner.SessionInfo.Name);
                //players[player].SetHealthBarColor(Color.green);
            }
            else
            {
                // Handle remote player
                StartCoroutine(WaitForPlayerObject(runner, player));
            }
        }
        else
        {
            PlayerController playerObject = runner.Spawn(playerControllerPrefab, new Vector3(0, 0, 0), Quaternion.identity, player);
            runner.SetPlayerObject(runner.LocalPlayer, playerObject.Object);
        }

    }

    private IEnumerator WaitForPlayerObject(NetworkRunner runner, PlayerRef player)
    {
        NetworkObject playerObject = null;
        float timeout = 5f; // 5 seconds timeout
        float elapsedTime = 0f;

        while (playerObject == null && elapsedTime < timeout)
        {
            playerObject = runner.GetPlayerObject(player);
            if (playerObject != null)
            {
                players[player] = playerObject.GetComponent<PlayerController>();
                Debug.Log($"Remote player {player} added to players list");
                Debug.Log("Players dictionanry" + players.Count);
                //UpdatePlayButtonInteractability();
                //players[player].SetRoomID(runner.SessionInfo.Name);
                //players[player].SetHealthBarColor(Color.blue);
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (playerObject == null)
        {
            Debug.LogWarning($"Timeout waiting for player object for player {player}");
        }
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        //throw new NotImplementedException();
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        //throw new NotImplementedException();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        //throw new NotImplementedException();
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        //throw new NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        //throw new NotImplementedException();
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        //throw new NotImplementedException();
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        //throw new NotImplementedException();
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        //throw new NotImplementedException();
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        //throw new NotImplementedException();
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        //throw new NotImplementedException();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        localPlayer.SetActive(true);
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        //throw new NotImplementedException();
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        //throw new NotImplementedException();
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        //throw new NotImplementedException();
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        //throw new NotImplementedException();
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        //throw new NotImplementedException();
        Debug.Log("Session list updated!");

        // Clear the existing list display
        //sessionListText.text = "Available Sessions:\n";

        // Loop through available sessions and display them
        foreach (var session in sessionList)
        {
            //sessionListText.text += $"Session Name: {session.Name}, Player Count: {session.PlayerCount}/{session.MaxPlayers}\n";
            string roomName = session.Name;
            int playerCount = session.PlayerCount;
            int maxPlayer = session.MaxPlayers;

            UIController.Instance.CreateRoomUI(roomName, playerCount, maxPlayer);
        }
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        //throw new NotImplementedException();
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        //throw new NotImplementedException();
    }
}
