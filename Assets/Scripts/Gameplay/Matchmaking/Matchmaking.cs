using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class Matchmaking : Fusion.Behaviour, INetworkRunnerCallbacks
{
    public static Matchmaking Instance;
    [SerializeField] private NetworkRunner networkRunnerPrefab;
    [SerializeField] private PlayerRoomController playerControllerPrefab;
    [SerializeField] private List<Transform> memberPos = new();
    [SerializeField] private Button readyButton;
    [SerializeField] private Button playButton;
    private NetworkRunner networkRunner;
    private const int MAX_PLAYER = 3;
    public GameObject localPlayer;
    public Dictionary<PlayerRef, PlayerRoomController> players = new();
    private PlayerRoomController localPlayerRoomController;
    private Vector3 spawnPosition;
    public Mode currentMode = Mode.Solo;
    private bool isAutoMatch;
    public int alivePlayer;
    bool isDone = false;
    public bool IsDone{get => isDone;}
    private PlayerRoomController localSoloPlayer;
    public Dictionary<PlayerRef, string> matchSolo = new();
    public bool IsAutoMatch
    {
        get { return isAutoMatch; }
        set
        {
            if (isAutoMatch != value)
            {
                isAutoMatch = value;
                if (localPlayerRoomController != null)
                    localPlayerRoomController.SetAutoMatch(isAutoMatch);
            }
        }
    }

    enum SceneBuildIndex
    {
        PlayScene = 2,
    }

    public int currentSceneIndex = 2; // This variable will hold the current scene index


    public enum Mode
    {
        Solo,
        Duo
    }

    private void Awake()
    {
        if (FindObjectsOfType<Matchmaking>().Length > 1)
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
        readyButton.onClick.AddListener(ToggleReady);
        
        GotoLobby();
    }


    public void SetSoloMode()
    {
        currentMode = Mode.Solo;
    }

    public void SetDuoMode()
    {
        currentMode = Mode.Duo;
    }

    public void ToggleReady()
    {
        PlayerRoomController localPlayer = players.Values.FirstOrDefault(p => p.Object.HasInputAuthority);
        if (localPlayer != null)
        {
            localPlayer.ToggleReady();
            readyButton.GetComponentInChildren<TextMeshProUGUI>().text = localPlayer.IsReady ? "CANCEL" : "READY";
        }
    }

    public void UpdatePlayButtonInteractability()
    {
        Debug.Log("Update play button interactablility");
        bool allPlayersReady = players.Values.All(p => p.IsReady || p.IsRoomOwner);
        playButton.interactable = allPlayersReady;
    }

    public void UpdateMap(string map)
    {
        if (currentMode != Mode.Duo) return;
        PlayerRoomController localPlayer = players.Values.FirstOrDefault(p => !p.Object.HasInputAuthority);
        if (localPlayer != null)
        {
            localPlayer.UpdateMap(map);
        }
    }

    public void UpdateMapProperty(string map)
    {
        if (map == "Harbour")
        {
            SetPlayScene(2);
            FindObjectOfType<UIController>().SetMapText("Map Harbour");
        }
        else if (map == "Desert")
        {
            SetPlayScene(3);
            FindObjectOfType<UIController>().SetMapText("Map Desert");
        }
    }

    public async void GotoLobby()
    {
        await JoinLobby();
    }

    public async Task JoinLobby()
    {
        if (networkRunner == null)
        {
            networkRunner = Instantiate(networkRunnerPrefab);
            networkRunner.AddCallbacks(this);
        }
        UIController.Instance.ShowHideUI(UIController.Instance.loadingPanel);
        await networkRunner.JoinSessionLobby(SessionLobby.Shared);
        UIController.Instance.ShowHideUI(UIController.Instance.loadingPanel);
        isDone = false;
        playButton.interactable = true;
        currentMode = Mode.Solo;
        players.Clear();
    }

    public void StartGame()
    {
        if (currentMode == Mode.Solo)
            QuickPlay();
        else
            StartCoroutine(QuickPlayTeam());
    }

    public async void QuickPlay()
    {
        if (networkRunner == null)
        {
            networkRunner = Instantiate(networkRunnerPrefab);
            networkRunner.AddCallbacks(this);
        }
        UIController.Instance.ShowHideUI(UIController.Instance.loadingPanel);
        var sceneInfo = new NetworkSceneInfo();
        Dictionary<string, SessionProperty> customProps = new();
        customProps["map"] = currentSceneIndex switch
        {
            2 => "Harbour",
            3 => "Middle East",
            _ => "Harbour",
        };
        customProps["type"] = "Survival Solo";
        sceneInfo.AddSceneRef(SceneRef.FromIndex(currentSceneIndex));
        var result = await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            Scene = sceneInfo,
            PlayerCount = MAX_PLAYER,
            SessionProperties = customProps,
        });

        if (result.Ok)
        {
            UIController.Instance.ShowHideUI(UIController.Instance.mainLobbyPanel);
            localPlayer.gameObject.SetActive(false);
            // all good
            Debug.Log("Match room name: " + networkRunner.SessionInfo.Name);

        }
        else
        {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }
        UIController.Instance.ShowHideUI(UIController.Instance.loadingPanel);
    }

    public IEnumerator QuickPlayTeam()
    {
        networkRunner.RemoveCallbacks(this);
        if (networkRunner.IsSharedModeMasterClient)
        {
            UIController.Instance.ShowHideUI(UIController.Instance.loadingPanel);
            // Set battle room for non-owner players
            foreach (var player in players.Where(p => p.Value.Object.HasInputAuthority == false))
            {
                //Debug.Log("Set battle name ne");
                player.Value.SetBattleRoom();
                yield return new WaitForSeconds(3f); // 3 seconds delay
            }

            if (networkRunner != null && networkRunner.IsRunning)
            {
                //await networkRunner.Shutdown();
                yield return networkRunner.Shutdown();
                //Destroy(networkRunner.gameObject);  // Destroy the runner instance
                networkRunner = null;  // Set the reference to null
                Debug.Log("Room owner leaved team room");
            }

            FindObjectOfType<MatchmakingTeam>().StartGame();

        }
    }

    public async void RPC_TransitionAllToBattleRoom()
    {
        UIController.Instance.ShowHideUI(UIController.Instance.loadingPanel);

        networkRunner.RemoveCallbacks(this);
        if (networkRunner != null && networkRunner.IsRunning)
        {
            await networkRunner.Shutdown();
            Destroy(networkRunner.gameObject);  // Destroy the runner instance
            networkRunner = null;  // Set the reference to null
            Debug.Log("Leaved team room");
        }
 
        FindObjectOfType<MatchmakingTeam>().StartGame();
    }

    public async void CreateRoom()
    {
        if (currentMode == Mode.Duo) return;
        players.Clear();    
        string teamcode = UnityEngine.Random.Range(100, 999).ToString();
        if (networkRunner == null)
        {
            networkRunner = Instantiate(networkRunnerPrefab);
            networkRunner.AddCallbacks(this);
        }
        UIController.Instance.ShowHideUI(UIController.Instance.loadingPanel);
        currentMode = Mode.Duo;
        Dictionary<string, SessionProperty> customProps = new();
        customProps["map"] = currentSceneIndex switch
        {
            2 => "Harbour",
            3 => "Desert",
            _ => "Harbour",
        };
        var startArguments = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = teamcode,
            PlayerCount = MAX_PLAYER,
            SessionProperties = customProps,
        };

        var result = await networkRunner.StartGame(startArguments);

        if (result.Ok)
        {
            Debug.Log("Team room name: " + networkRunner.SessionInfo.Name);
            UIController.Instance.OnOffPanel();
        }

        UIController.Instance.ShowHideUI(UIController.Instance.loadingPanel);
    }

    public void UpdateSessionProperties()
    {
        if (networkRunner == null || !networkRunner.IsRunning)
            return;

        if (currentMode == Mode.Solo) return;

        try
        {
            Dictionary<string, SessionProperty> newProps = new();
            newProps["map"] = currentSceneIndex switch
            {
                2 => "Harbour",
                3 => "Desert",
                _ => "Harbour",
            };
            // Update the custom properties
            networkRunner.SessionInfo.UpdateCustomProperties(newProps);
            Debug.Log("Session properties updated successfully");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error updating session properties: {e.Message}");
        }
    }

    public async void LeaveRoom()
    {
        if (currentMode == Mode.Solo) return;
        UIController.Instance.ShowHideUI(UIController.Instance.loadingPanel);
        if (networkRunner != null)
        {
            Debug.Log("Leaving room...");
            if (players[networkRunner.LocalPlayer].IsRoomOwner)
            {
                PlayerRoomController localPlayer = players.Values.FirstOrDefault(p => p.Object.HasInputAuthority == false);
                if (localPlayer != null)
                {
                    localPlayer.RPC_SetAsRoomOwner();
                }
            }
            await networkRunner.Shutdown();

            Debug.Log("You have left the room.");
            currentMode = Mode.Solo;
            UIController.Instance.SwitchMode(true);
            networkRunner = null;
            readyButton.gameObject.SetActive(false);
            playButton.gameObject.SetActive(true);
            playButton.interactable = true;
            localPlayer.SetActive(true);
            UIController.Instance.SwitchMode(true);
            UIController.Instance.OnOffPanel();
            UIController.Instance.ShowHideUI(UIController.Instance.loadingPanel);
            UIController.Instance.AllowSelectMapMode(true);
            await JoinLobby();
        }
        else
        {
            Debug.LogWarning("Network runner is not initialized or you are not in a session.");
        }
    }

    public async void BackToLobby()
    {
        await networkRunner.Shutdown();
        SceneManager.LoadScene("MainLobby");
        isDone = false;
        UIController.Instance.ShowHideUI(UIController.Instance.mainLobbyPanel);
        localPlayer.gameObject.SetActive(true);
        await JoinLobby();
    }

    public void BackToLobbyAll()
    {
        if (currentMode == Mode.Solo)
            BackToLobby();
        else
            MatchmakingTeam.Instance.BackToLobby();
    }

    public async void JoinRoomByName(string roomName)
    {
        currentMode = Mode.Duo;
        players.Clear();
        UIController.Instance.ShowHideUI(UIController.Instance.loadingPanel);
        if (networkRunner == null)
        {
            networkRunner = Instantiate(networkRunnerPrefab);
            networkRunner.AddCallbacks(this);
        }
        var result = await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = roomName,
            PlayerCount = MAX_PLAYER,
        });

        if (result.Ok)
        {
            UIController.Instance.SwitchMode(false);
            UIController.Instance.OnOffPanel();
    }
        else
        {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }
        UIController.Instance.ShowHideUI(UIController.Instance.loadingPanel);
    }

    public async void RejoinRoomByName(string roomName)
    {
        currentMode = Mode.Duo;
        players.Clear();
        UIController.Instance.ShowHideUI(UIController.Instance.loadingPanel);
        if (networkRunner == null)
        {
            networkRunner = Instantiate(networkRunnerPrefab);
            networkRunner.AddCallbacks(this);
        }
        Dictionary<string, SessionProperty> customProps = new();
        customProps["map"] = currentSceneIndex switch
        {
            2 => "Harbour",
            3 => "Desert",
            _ => "Harbour",
        };

        var result = await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = roomName,
            PlayerCount = MAX_PLAYER,
            SessionProperties = customProps,
        });

        if (result.Ok)
        {
            UIController.Instance.SwitchMode(false);
            UIController.Instance.OnOffPanel();
        }
        else
        {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }
        UIController.Instance.ShowHideUI(UIController.Instance.loadingPanel);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        
        if (currentMode == Mode.Duo)
        {
            localPlayer.SetActive(false);
            if (player == runner.LocalPlayer)
            {
                spawnPosition = memberPos[runner.ActivePlayers.Count() - 1].position;
                PlayerRoomController playerObject = runner.Spawn(playerControllerPrefab, spawnPosition, Quaternion.identity, player);
                runner.SetPlayerObject(runner.LocalPlayer, playerObject.Object);
                players[player] = playerObject.GetComponent<PlayerRoomController>();
                players[player].TurnOnTeamMemberPanel();
                players[player].SetRoomID(runner.SessionInfo.Name);
                players[player].SetAutoMatch(isAutoMatch);
                localPlayerRoomController = players[player];
            }
            else
            {
                // Handle remote player
                StartCoroutine(WaitForPlayerObject(runner, player));
            }

            if (runner.IsSharedModeMasterClient && player == runner.LocalPlayer)
            {
                players[player].RPC_SetAsRoomOwner();
                playButton.gameObject.SetActive(true);
                readyButton.gameObject.SetActive(false);
                UpdatePlayButtonInteractability();
            }
            else if (player == runner.LocalPlayer)
            {
                players[player].SetAsRoomMember();
                readyButton.gameObject.SetActive(true);
                playButton.gameObject.SetActive(false);
                readyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ready";
                UIController.Instance.AllowSelectMapMode(false);
            }

            if (runner.IsSharedModeMasterClient)
                UpdatePlayButtonInteractability();
        }
        else
        {
            if (player == runner.LocalPlayer)
            {
                PlayerRoomController playerObject = runner.Spawn(playerControllerPrefab, new Vector3(0, 0, 0), Quaternion.identity, player);
                runner.SetPlayerObject(runner.LocalPlayer, playerObject.Object);
                localSoloPlayer = playerObject.GetComponent<PlayerRoomController>();
                playerObject.GetComponent<PlayerRoomController>().SetPlayerRef(player);
                playerObject.GetComponent<PlayerRoomController>().SetTeamID(runner.UserId);
                playerObject.GetComponent<PlayerRoomController>().SetLocalPlayer();
                players[player] = playerObject.GetComponent<PlayerRoomController>();
                if (players.Count == MAX_PLAYER)
                {
                    Debug.Log("=== Start battle.......");
                    StartBattle();
                    Debug.Log("Start local!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

                    players.Clear();

                }
                matchSolo[player] = players[player].TeamID.ToString();
            }
            else
            {
                // Handle remote player
                StartCoroutine(WaitForPlayerObjectSolo(runner, player));
            }
            int remainPlayer = MAX_PLAYER - networkRunner.ActivePlayers.Count();
            string text = "Waiting other player: " + remainPlayer + " remain";
            FindObjectOfType<UIController>().SetText(text);
        }
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
        FindObjectOfType<WaitingArea>()?.ReleasePlayer();


    }

    private IEnumerator InitializeTeams()
    {
        yield return new WaitForSeconds(6f);
        FindObjectOfType<GameHandler>().InitializeTeams();
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
                NetworkPlayer networkPlayer = players[player].GetComponent<NetworkPlayer>();
                networkPlayer.SetNicknameUIColor(Color.blue); //Set teammate name plate UI color to blue
                UpdatePlayButtonInteractability();
                players[player].SetRoomID(runner.SessionInfo.Name);
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

    private IEnumerator WaitForPlayerObjectSolo(NetworkRunner runner, PlayerRef player)
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
                NetworkPlayer networkPlayer = players[player].GetComponent<NetworkPlayer>();
                networkPlayer.SetNicknameUIColor(Color.red); //Set enemy name plate UI color to red
                matchSolo[player] = players[player].TeamID.ToString();
                if (players.Count == MAX_PLAYER)
                {
                    Debug.Log("=== Start battle.......");
                    StartBattle();
                    Debug.Log("Start remote!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
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
        if (currentMode == Mode.Solo && !isDone)
        {
            int remainPlayer = MAX_PLAYER - networkRunner.ActivePlayers.Count();
            string text = "Waiting other player: " + remainPlayer + " remain";
            FindObjectOfType<UIController>().SetText(text);
        }

        if (currentMode == Mode.Solo && isDone)
        {
            FindObjectOfType<GameHandler>().Eliminate(matchSolo[player], players[player]);
            FindObjectOfType<GameHandler>().CheckWin();
        }

        players.Remove(player);

        // Setup when team member become room owner when room owner left room
        if (player != runner.LocalPlayer && currentMode == Mode.Duo)
            players[runner.LocalPlayer].RPC_SetAsRoomOwner();

        // Setup when team member become room owner
        if (players.ContainsKey(runner.LocalPlayer) && currentMode == Mode.Duo)
        {
            if (players[runner.LocalPlayer].IsRoomOwner)
            {
                players[runner.LocalPlayer].gameObject.transform.position = memberPos[0].position;
                readyButton.gameObject.SetActive(false);
                playButton.gameObject.SetActive(true);
                UIController.Instance.AllowSelectMapMode(true);
            }
            UpdatePlayButtonInteractability();
        }   
    }


    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        UIController.Instance.ClearSessionButtons();
        // Loop through available sessions and display them
        foreach (var session in sessionList)
        {
            string roomName = session.Name;
            int playerCount = session.PlayerCount;
            int maxPlayer = session.MaxPlayers;
            // Get map from custom properties
            string map = "Unknown";
            if (session.Properties.TryGetValue("map", out var mapValue))
            {
                map = mapValue.PropertyValue.ToString();
            }
            if (session.IsOpen && roomName.Length <= 3)
                UIController.Instance.CreateRoomUI(roomName, playerCount, maxPlayer, map);
        }
    }

    public void SetPlayScene(int sceneIndex)
    {
        // You can add validation here if needed
        currentSceneIndex = sceneIndex;
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

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        //throw new NotImplementedException();
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        //throw new NotImplementedException();
    }
}
