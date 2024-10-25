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
    [SerializeField] private GameObject localPlayer;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button playButton;
    private NetworkRunner networkRunner;
    private const int MAX_PLAYER = 3;
    public Dictionary<PlayerRef, PlayerRoomController> players = new();
    private PlayerRoomController localPlayerRoomController;
    private Vector3 spawnPosition;
    public Mode currentMode = Mode.Solo;
    private bool isAutoMatch;
    public int alivePlayer;
    private PlayerRoomController localSoloPlayer;
    public bool IsAutoMatch
    {
        get { return isAutoMatch; }
        set
        {
            if (isAutoMatch != value) // Check if the value actually changed
            {
                isAutoMatch = value;
                if (localPlayerRoomController != null)
                    localPlayerRoomController.SetAutoMatch(isAutoMatch);
            }
        }
    }
   
    //private const int TEAM_SIZE = 2;
    enum SceneBuildIndex
    {
        PlayScene = 2,
    }

    public enum Mode
    {
        Solo,
        Duo
    }

    private void Awake()
    {
        // Check if there is already a canvas with this tag to avoid duplicates
        if (FindObjectsOfType<Matchmaking>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        //// Make this GameObject persistent across scenes
        //DontDestroyOnLoad(gameObject);
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
        void Start()
    {
        readyButton.onClick.AddListener(ToggleReady);
        
        JoinLobby();
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
        //playButton.interactable = !playButton.interactable;

    }

    public async void JoinLobby()
    {
        if (networkRunner == null)
        {
            networkRunner = Instantiate(networkRunnerPrefab);
            networkRunner.AddCallbacks(this);
        }
        // Call this to join the session lobby
        await networkRunner.JoinSessionLobby(SessionLobby.Shared);
    }

    public void StartGame()
    {
        if (currentMode == Mode.Solo)
            QuickPlay();
        else
            QuickPlayTeam();
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
            UIController.Instance.ShowHideUI(UIController.Instance.mainLobbyPanel);
            localPlayer.gameObject.SetActive(false);
            // all good
            Debug.Log("Match room name: " + networkRunner.SessionInfo.Name);

        }
        else
        {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }
    }

    public async void QuickPlayTeam()
    {
        networkRunner.RemoveCallbacks(this);
        if (networkRunner.IsSharedModeMasterClient)
        {
            // Set battle room for non-owner players
            foreach (var player in players.Where(p => p.Value.Object.HasInputAuthority == false))
            {
                //Debug.Log("Set battle name ne");
                player.Value.SetBattleRoom();
                await Task.Delay(3000);
            }
            //TransitionToBattleRoom();
            //networkRunner.RemoveCallbacks(this);
            // Leave current team room
            //playButton.gameObject.SetActive(false);
            if (networkRunner != null && networkRunner.IsRunning)
            {
                await networkRunner.Shutdown();
                //Destroy(networkRunner.gameObject);  // Destroy the runner instance
                networkRunner = null;  // Set the reference to null
                Debug.Log("Room owner leaved team room");
            }
            // Ensure that the runner is no longer running before starting the next session
            //if (!networkRunner.IsRunning)
            {
                FindObjectOfType<MatchmakingTeam>().StartGame();
            }

        }
    }

    public async void RPC_TransitionAllToBattleRoom()
    {
        Debug.Log("Team member join battle room");
        playButton.gameObject.SetActive(false);
        readyButton.gameObject.SetActive(false);
        //TransitionToBattleRoom();

        networkRunner.RemoveCallbacks(this);
        if (networkRunner != null && networkRunner.IsRunning)
        {
            await networkRunner.Shutdown();
            Destroy(networkRunner.gameObject);  // Destroy the runner instance
            networkRunner = null;  // Set the reference to null
            Debug.Log("Leaved team room");
        }
        //if (!networkRunner.IsRunning)
        {
            FindObjectOfType<MatchmakingTeam>().StartGame();
        }
    }

    public async void CreateRoom()
    {
        players.Clear();
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
            Debug.Log("Team room name: " + networkRunner.SessionInfo.Name);
            UIController.Instance.OnOffPanel();
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
            if (players[networkRunner.LocalPlayer].IsRoomOwner)
            {
                PlayerRoomController localPlayer = players.Values.FirstOrDefault(p => p.Object.HasInputAuthority == false);
                if (localPlayer != null)
                {
                   localPlayer.RPC_SetAsRoomOwner();
                }
            }

            // Waiting for setup new room owner if needed
            await Task.Delay(1000);
            // Shuts down the runner and leaves the current session
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
            JoinLobby();

            // Optionally update the UI, e.g., re-enable room creation UI or show session list
            // FindObjectOfType<UIManager>().TurnOnCreateRoomButton();
            // sessionListContent.parent.parent.gameObject.SetActive(true);
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
        UIController.Instance.ShowHideUI(UIController.Instance.mainLobbyPanel);
        localPlayer.gameObject.SetActive(true);
        JoinLobby();
    }

    public async void JoinRoomByName(string roomName)
    {
        currentMode = Mode.Duo;
        players.Clear();
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
            UIController.Instance.SwitchMode(false);
            UIController.Instance.OnOffPanel();
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
                PlayerRoomController playerObject = runner.Spawn(playerControllerPrefab, spawnPosition, Quaternion.identity, player);
                runner.SetPlayerObject(runner.LocalPlayer, playerObject.Object);
                players[player] = playerObject.GetComponent<PlayerRoomController>();
                Debug.Log("New player joined " + player.ToString());
                Debug.Log("Player count " + runner.ActivePlayers.Count());
                players[player].TurnOnTeamMemberPanel();
                players[player].SetRoomID(runner.SessionInfo.Name);
                players[player].SetAutoMatch(isAutoMatch);
                localPlayerRoomController = players[player];
               
                //players[player].SetHealthBarColor(Color.green);

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
                UpdatePlayButtonInteractability();
            }
            else if (player == runner.LocalPlayer)
            {
                players[player].SetAsRoomMember();
                readyButton.gameObject.SetActive(true);
                readyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ready";
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
            }
            int remainPlayer = MAX_PLAYER - runner.ActivePlayers.Count();
            string text = "Waiting other player: " + remainPlayer + " remain";
            FindObjectOfType<UIController>().SetText(text);
            if (runner.ActivePlayers.Count() == MAX_PLAYER) // Assuming PlayerCount is 2
            {
                alivePlayer = runner.ActivePlayers.Count();
                FindObjectOfType<UIController>().StartCountdown();
                StartCoroutine(ReleasePlayer());
            }
        }
    }

    private IEnumerator ReleasePlayer()
    {
        yield return new WaitForSeconds(4f);
        FindObjectOfType<WaitingArea>()?.ReleasePlayer();
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
                Debug.Log($"Remote player {player} added to players list");
                Debug.Log("Players dictionanry" + players.Count);
                //players[player].TurnOnTeamMemberPanel();
                UpdatePlayButtonInteractability();
                players[player].SetRoomID(runner.SessionInfo.Name);
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

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        players.Remove(player);
        // Setup when team member become room owner
        if (players.ContainsKey(runner.LocalPlayer))
        {
            if (players[runner.LocalPlayer].IsRoomOwner)
            {
                players[runner.LocalPlayer].gameObject.transform.position = memberPos[0].position;
                readyButton.gameObject.SetActive(false);
            }
            UpdatePlayButtonInteractability();
        }

        //localPlayerRoomController = null;
    }


    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        //throw new NotImplementedException();
        Debug.Log("Session list updated!");

        // Clear the existing list display
        //sessionListText.text = "Available Sessions:\n";

        UIController.Instance.ClearSessionButtons();
        // Loop through available sessions and display them
        foreach (var session in sessionList)
        {
            string roomName = session.Name;
            int playerCount = session.PlayerCount;
            int maxPlayer = session.MaxPlayers;

            UIController.Instance.CreateRoomUI(roomName, playerCount, maxPlayer);
        }
    }

    public void CheckWin(PlayerRef player)
    {
        alivePlayer--;
        Debug.Log("Check win ne");
        if (alivePlayer == 1 && player != networkRunner.LocalPlayer)
        {
            Debug.Log("WINNNNN!!!!");
            localSoloPlayer.GetComponent<NetworkPlayer>().localUI.SetActive(false);
            FindObjectOfType<WorldUI>().ShowHideWinUI();
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
