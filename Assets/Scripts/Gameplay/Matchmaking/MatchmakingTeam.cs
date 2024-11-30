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
    public static MatchmakingTeam Instance;
    public Dictionary<PlayerRef, PlayerRoomController> players = new();
    public TextMeshProUGUI StatusText;
    public bool IsDone { get => isDone; }
    public Dictionary<string, List<PlayerRef>> teams = new();
    public Dictionary<PlayerRef, string> matchTeam = new();

    [SerializeField] private NetworkRunner networkRunnerPrefab;
    [SerializeField] private PlayerRoomController playerControllerPrefab;
    [SerializeField] private int MAX_PLAYER = 3;

    private NetworkRunner networkRunner;
    private string roomID;
    private string teamID = "";
    private string roomAutoMatch = "";
    private bool isDone = false;
    private enum SceneBuildIndex
    {
        PlayScene = 2,
    }

    private void Awake()
    {
        if (FindObjectsOfType<MatchmakingTeam>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public async void StartGame()
    {
        isDone = false;
        players.Clear();
        if (networkRunner == null)
        {
            networkRunner = Instantiate(networkRunnerPrefab);
            networkRunner.AddCallbacks(this);
        }
        int currentSceneIndex = Matchmaking.Instance.currentSceneIndex;
        Dictionary<string, SessionProperty> customProps = new();
        customProps["type"] = "Survival Team";
        customProps["map"] = currentSceneIndex switch
        {
            2 => "Harbour",
            3 => "Desert",
            _ => "Harbour",
        };
        var sceneInfo = new NetworkSceneInfo();
        sceneInfo.AddSceneRef(SceneRef.FromIndex(currentSceneIndex));
        var result = await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,      
            Scene = sceneInfo,
            SessionProperties = customProps,
            PlayerCount = MAX_PLAYER,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
        });

        if (result.Ok)
        {
            UIController.Instance.ShowHideUI(UIController.Instance.mainLobbyPanel);
            UIController.Instance.ShowHideUI(UIController.Instance.loadingPanel);          
        }
        else
        {
            Debug.LogError("Failed to join battle room: " + result.ShutdownReason);
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        roomID = PlayerPrefs.GetString("RoomID");
        
        if (player == runner.LocalPlayer)
        {
            PlayerRoomController playerObject = runner.Spawn(playerControllerPrefab, new Vector3(0, 0, 0), Quaternion.identity, player);
            runner.SetPlayerObject(runner.LocalPlayer, playerObject.Object);  
            players[player] = playerObject.GetComponent<PlayerRoomController>();
            players[player].SetRoomID(roomID);
            players[player].SetLocalPlayer();
            matchTeam[player] = players[player].TeamID.ToString();
            int IsRoomOwner = PlayerPrefs.GetInt("IsRoomOwner");
            if (IsRoomOwner == 1)
            {
                players[player].RPC_SetAsRoomOwner();
                players[player].IsRoomOwner = true;
            }
            else
            {
                players[player].SetAsRoomMember();
                players[player].IsRoomOwner = false;
            }

            int isAutoMatch = PlayerPrefs.GetInt("IsAutoMatch");
            if (isAutoMatch == 1)
            {
                players[player].SetAutoMatch(true);
            }
            else
            {
                players[player].SetAutoMatch(false);
            }

            if (players[player].IsAutoMatch)
            {
                if (runner.IsSharedModeMasterClient)
                {
                    roomAutoMatch = GenerateRoomName();
                    players[player].SetTeamID(roomAutoMatch);
                    if (teams.ContainsKey(roomAutoMatch))
                    {
                        teams[roomAutoMatch].Add(player);
                    }
                    else
                    {
                        List<PlayerRef> listPlayers = new();
                        listPlayers.Add(player);
                        teams[roomAutoMatch] = listPlayers;
                    }
                }
            }
            else
            {
                players[player].SetTeamID(roomID);
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

            teamID = players[player].TeamID.ToString();

            if (players.Count == MAX_PLAYER)
            {
                Debug.Log("=== Start battle.......");
                StartBattle();
            }
        }
        else
        {
            // Handle remote player
            StartCoroutine(WaitForPlayerObject(runner, player));
        }

        int remainPlayer = MAX_PLAYER - runner.ActivePlayers.Count();
        string text = "Waiting other player: " + remainPlayer + " remain";   
        FindObjectOfType<UIController>().SetText(text);
    }

    public void StartBattle()
    {
        networkRunner.SessionInfo.IsOpen = false;
        isDone = true;
        FindObjectOfType<UIController>().StartCountdown();
        StartCoroutine(ReleasePlayer());
        StartCoroutine(InitializeTeams());
    }

    private IEnumerator ReleasePlayer()
    {
        yield return new WaitForSeconds(4f);
        FindObjectOfType<WaitingArea>().ReleasePlayer();   
    }

    private IEnumerator InitializeTeams()
    {
        yield return new WaitForSeconds(8f);
        FindObjectOfType<GameHandler>().InitializeTeams();
    }

    public string GenerateRoomName()
    {
        string roomAutoMatch;
        do
        {
            // Generate a random room name
            roomAutoMatch = "AutoMatch" + UnityEngine.Random.Range(0, 10);
        }
        // Continue generating until the room name is either not in the dictionary or has fewer than 2 players
        while (teams.ContainsKey(roomAutoMatch) && teams[roomAutoMatch].Count >= 2);

        return roomAutoMatch;
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
                matchTeam[player] = players[player].TeamID.ToString();
                if (players[player].IsAutoMatch)
                {
                    if (runner.IsSharedModeMasterClient)
                    {
                        if (teams.ContainsKey(roomAutoMatch) && teams[roomAutoMatch].Count >= 2 || roomAutoMatch == "")
                            roomAutoMatch = GenerateRoomName();
                        players[player].SetTeamID(roomAutoMatch);
                        if (teams.ContainsKey(roomAutoMatch))
                        {
                            teams[roomAutoMatch].Add(player);
                        }
                        else
                        {
                            List<PlayerRef> listPlayers = new();
                            listPlayers.Add(player);
                            teams[roomAutoMatch] = listPlayers;
                        }
                    }        
                }
                else //Not auto match
                {
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

                if (players.Count == MAX_PLAYER)
                {
                    Debug.Log("=== Start battle.......");
                    StartBattle();
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

    public async void BackToLobby()
    {
        await networkRunner.Shutdown();
        SceneManager.LoadScene("MainLobby");
        UIController.Instance.ShowHideUI(UIController.Instance.mainLobbyPanel);
        Matchmaking.Instance.localPlayer.gameObject.SetActive(true);
        UIController.Instance.ResetUI();
        await Matchmaking.Instance.JoinLobby();
        Matchmaking.Instance.RejoinRoomByName(roomID);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (!isDone)
        {
            int remainPlayer = MAX_PLAYER - runner.ActivePlayers.Count();
            string text = "Waiting other player: " + remainPlayer + " remain";
            FindObjectOfType<UIController>().SetText(text);
        }

        if (player == runner.LocalPlayer)
            players.Clear();
        string team = matchTeam[player];
        PlayerRoomController playerRoom = players[player];
        FindObjectOfType<GameHandler>().Eliminate(team, playerRoom);
        if (runner.ActivePlayers.Count() > 1)
            FindObjectOfType<GameHandler>().CheckWin();
        players.Remove(player);
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
