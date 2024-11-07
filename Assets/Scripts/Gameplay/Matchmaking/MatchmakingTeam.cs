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
    [SerializeField] private NetworkRunner networkRunnerPrefab;
    [SerializeField] private PlayerRoomController playerControllerPrefab;
    private NetworkRunner networkRunner;
    public Dictionary<PlayerRef, PlayerRoomController> players = new();
    public TextMeshProUGUI StatusText;
    private const int MAX_PLAYER = 2;
    string roomID;
    string teamID = "";
    string roomAutoMatch = "";
    bool isDone = false;
    public bool IsDone { get => isDone; }
    //[SerializeField] private GameHandler gameManagerPrefab;
    //private GameHandler gameManager;

    public Dictionary<string, List<PlayerRef>> teams = new();
    public Dictionary<PlayerRef, string> matchTeam = new();
    enum SceneBuildIndex
    {
        PlayScene = 2,
    }

    private void Awake()
    {
        // Check if there is already a canvas with this tag to avoid duplicates
        if (FindObjectsOfType<MatchmakingTeam>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        // Check if instance already exists and destroy if duplicate
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            // Set the instance to this object
            Instance = this;
            // Optionally, make the object persistent across scenes
            DontDestroyOnLoad(gameObject);
        }
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
        isDone = false;
        players.Clear();
        Debug.Log("=== Start new game -> clear players");
        if (networkRunner == null)
        {
            networkRunner = Instantiate(networkRunnerPrefab);
            networkRunner.AddCallbacks(this);
        }
        int currentSceneIndex = Matchmaking.Instance.currentSceneIndex;
        Dictionary<string, SessionProperty> customProps = new();
        //customProps["map"] = "Test";
        customProps["type"] = "Survival Team";
        customProps["map"] = currentSceneIndex switch
        {
            2 => "Harbour",
            3 => "Desert",
            _ => "Harbour",
        };
        var sceneInfo = new NetworkSceneInfo();
        //int playSceneIndex = (int)SceneBuildIndex.PlayScene;
        sceneInfo.AddSceneRef(SceneRef.FromIndex(currentSceneIndex));
        var result = await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            //SessionName = "Battle",
            Scene = sceneInfo, // Assuming you have a separate battle room scene
            SessionProperties = customProps,
            PlayerCount = MAX_PLAYER,// Adjust based on your team sizes
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
        });

        if (result.Ok)
        {
            UIController.Instance.ShowHideUI(UIController.Instance.mainLobbyPanel);
            UIController.Instance.ShowHideUI(UIController.Instance.loadingPanel);
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
            players[player].SetLocalPlayer();
            matchTeam[player] = players[player].TeamID.ToString();
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

        //Debug.Log("______PLAYER COUNT: " + runner.ActivePlayers.Count());
        int remainPlayer = MAX_PLAYER - runner.ActivePlayers.Count();
        string text = "Waiting other player: " + remainPlayer + " remain";
        
        FindObjectOfType<UIController>().SetText(text);
        if (runner.ActivePlayers.Count() == MAX_PLAYER && !isDone) // Assuming PlayerCount is 2
        {
            Debug.Log("===Start battle old....");
            //isDone = true;
            //runner.SessionInfo.IsOpen = false;
            //FindObjectOfType<UIController>().StartCountdown();
            //StartCoroutine(ReleasePlayer());
            ////if (player == runner.LocalPlayer)
            //StartCoroutine(InitializeTeams());
        }
    }

    public void StartBattle()
    {
        Debug.Log("===Start battle thoi");
        networkRunner.SessionInfo.IsOpen = false;
        isDone = true;
        //alivePlayer = runner.ActivePlayers.Count();
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

    private IEnumerator CheckTeamMate(PlayerRef player)
    {
        yield return new WaitForSeconds(3);
        if (players[player].TeamID.ToString() == teamID)
        {
            Debug.Log("####DONG DOI VAO ROI NEEEEEEEEEE");
            players[player].SetTeamMateTag();
        }
        else
        {
            Debug.Log("####Local team id: " + teamID + "Player team id " + players[player].TeamID.ToString());
        }
    }

    private IEnumerator WaitForTeamID(NetworkRunner runner, PlayerRef player)
    {
        //NetworkObject playerObject = null;
        float timeout = 5f; // 5 seconds timeout
        float elapsedTime = 0f;
        while (teamID == "" && elapsedTime < timeout)
        {
            teamID = players[player].TeamID.ToString();
            if (teamID != "")
            {
                Debug.Log("******TEAM ID OF LOCAL PLAYER: " + teamID);
                yield break;
            }               
            elapsedTime += Time.deltaTime;
            yield return null;
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
        Matchmaking.Instance.JoinRoomByName(roomID);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        players.Remove(player);
        Debug.Log("===Player trong team left neeee");
        //throw new NotImplementedException();
        string team = matchTeam[player];
        PlayerRoomController playerRoom = players[player];
        FindObjectOfType<GameHandler>().Eliminate(team, playerRoom);
        //StartCoroutine(FindObjectOfType<GameHandler>().CheckLose(matchTeam[player]));
        FindObjectOfType<GameHandler>().CheckWin();
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
