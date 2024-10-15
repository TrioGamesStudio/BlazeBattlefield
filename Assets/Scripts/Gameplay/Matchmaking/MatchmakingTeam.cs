using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Fusion.Sockets;
using System;
using System.Linq;
using System.Threading.Tasks;

public class MatchmakingTeam : Fusion.Behaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkRunner networkRunnerPrefab;
    [SerializeField] private PlayerRoomController playerControllerPrefab;
    private NetworkRunner networkRunner;
    public Dictionary<PlayerRef, PlayerRoomController> players = new();
    public TextMeshProUGUI StatusText;
    private const int MAX_PLAYER = 4;
    string roomID;

    //[SerializeField] private GameHandler gameManagerPrefab;
    //private GameHandler gameManager;

    public Dictionary<string, List<PlayerRef>> teams = new();
    enum SceneBuildIndex
    {
        PlayScene = 2,
    }
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public async void StartGame()
    {
        if (networkRunner == null)
        {
            networkRunner = Instantiate(networkRunnerPrefab);
            networkRunner.AddCallbacks(this);
        }
        Dictionary<string, SessionProperty> customProps = new();

        customProps["map"] = "Test";
        customProps["type"] = "Survival";
        var sceneInfo = new NetworkSceneInfo();
        int playSceneIndex = (int)SceneBuildIndex.PlayScene;
        sceneInfo.AddSceneRef(SceneRef.FromIndex(playSceneIndex));
        var result = await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            //SessionName = "Battle",
            Scene = sceneInfo, // Assuming you have a separate battle room scene
            SessionProperties = customProps,
            PlayerCount = 4,// Adjust based on your team sizes
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
        });

        if (result.Ok)
        {
            Debug.Log("Joined team battle room: " + networkRunner.SessionInfo.Name);
        }
        else
        {
            Debug.LogError("Failed to join battle room: " + result.ShutdownReason);
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("New player join battle scene ne");
        roomID = PlayerPrefs.GetString("RoomID");
        if (player == runner.LocalPlayer)
        {
            Debug.Log("SPAWN PLAYER");
            PlayerRoomController playerObject = runner.Spawn(playerControllerPrefab, new Vector3(0, 0, 0), Quaternion.identity, player);
            runner.SetPlayerObject(runner.LocalPlayer, playerObject.Object);  
            players[player] = playerObject.GetComponent<PlayerRoomController>();
            players[player].SetRoomID(roomID);
            //players[player].SetHealthBarColor(Color.green);
            Debug.Log("New player joined " + player.ToString());
            Debug.Log("Player count " + runner.ActivePlayers.Count());
            int IsRoomOwner = PlayerPrefs.GetInt("IsRoomOwner");
            if (IsRoomOwner == 1)
            {
                Debug.Log("THIS IS TEAM ROOM OWNER");
                players[player].RPC_SetAsRoomOwner();
                players[player].IsRoomOwner = true;
            }
            else
            {
                Debug.Log("THIS IS TEAM ROOM MEMBER");
                players[player].SetAsRoomMember();
                players[player].IsRoomOwner = false;
            }

            if (teams.ContainsKey(players[player].RoomID.ToString()))
            {
                teams[players[player].RoomID.ToString()].Add(player);
            }
            else
            {
                List<PlayerRef> listPlayers = new();
                listPlayers.Add(player);
                teams[players[player].RoomID.ToString()] = listPlayers;
            }
        }
        else
        {
            // Handle remote player
            StartCoroutine(WaitForPlayerObject(runner, player));
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
                players[player] = playerObject.GetComponent<PlayerRoomController>();
                if (teams.ContainsKey(players[player].RoomID.ToString()))
                {
                    teams[players[player].RoomID.ToString()].Add(player);
                }
                else
                {
                    List<PlayerRef> listPlayers = new();
                    listPlayers.Add(player);
                    teams[players[player].RoomID.ToString()] = listPlayers;
                }
                //if (players[player].RoomID == roomID)
                {
                    //players[player].SetHealthBarColor(Color.blue);
                }
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

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        //throw new NotImplementedException();
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
