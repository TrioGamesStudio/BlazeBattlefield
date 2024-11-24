using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Fusion;
using TMPro;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft, IPlayerJoined
{
    // player name
    [Networked]
    public NetworkString<_16> nickName_Network{get; set;}
    bool isPublicJoinMessageSent = false;

    NetworkInGameMessages networkInGameMessages;
    [SerializeField] TextMeshProUGUI nickName_TM;
    [SerializeField] GameObject miniMap_Image;

    ChangeDetector changeDetector;

    public static NetworkPlayer Local { get; set; }
    public Transform playerModel;
    
    // camera
    [SerializeField] private LocalCameraHandler localCameraHandler;
    public LocalCameraHandler LocalCameraHandler => localCameraHandler;
    [SerializableType] MiniMapCameraHandler miniMapCameraHandler;

    // UI chua crossHair, red image get damage
    public GameObject localUI; // game object = PlayerUICanvas (canvas cua ca player)

    // TESTING PLAYER DATA LIST ACTIVED PLAYERS
    [Networked]
    [Capacity(10)] // Sets the fixed capacity of the collection
    [UnitySerializeField] // Show this private property in the inspector.
    NetworkDictionary<int, NetworkString<_32>> NetDict => default;

    Dictionary<int, string> LocalDict = new Dictionary<int, string>();
    
    // TEAM
    [SerializeField] bool isEnemy;
    public bool IsEnemy {get {return isEnemy;} set {isEnemy = value;}} // <- InitializeNetworkPlayerBeforeSpawn() spawner.cs

    [Networked]
    public NetworkBool isEnemy_Network{ get; set; } // <- RPC

    // Spanwer -> set this.networkRunner and this.scenetoStart
    /* NetworkRunner networkRunner;
    public NetworkRunner NetworkRunner{get => networkRunner;} */
    [SerializeField] string sceneToStart;
    public string SceneToStart { get => sceneToStart;}

    //Spawner spawner;
    NetworkRunner networkRunner;

    public string nickName;

    private void Awake() {
        localCameraHandler = GetComponentInChildren<LocalCameraHandler>();
        miniMapCameraHandler = GetComponentInChildren<MiniMapCameraHandler>();

        networkInGameMessages = GetComponent<NetworkInGameMessages>();
        /* spawner = FindObjectOfType<Spawner>(); */
        networkRunner = FindObjectOfType<NetworkRunner>();
        //Local = this;
        DontDestroyOnLoad(this.gameObject);
    }


    //? nhung thay doi cua bien Network
    public override void Render() {
        foreach (var change in changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(nickName_Network):
                    OnNickNameChanged();
                    break;
                case nameof(isEnemy_Network):
                    OnIsEnemyChanged();
                    break;
            }
        }
    }
    public override void Spawned()
    {
        Debug.Log($"_____co chay spawn()");
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        OnNickNameChanged();//? phai co de show ten khi spawn vao world1 scene
        OnIsEnemyChanged();
        
        //? khi spawn chan FixUpdateNetwork in CharactermovementHandler run - coll 45 -> player spawn
        if(Object.HasStateAuthority) {
            GetComponent<CharacterMovementHandler>().RequestRespawn();
        }

        // kiem tra co dang spawn tai ready scene hay khong
        bool isReadyScene = SceneManager.GetActiveScene().name == "MainLobby";

        if(this.Object.HasStateAuthority) {
            Local = this;
            // kiem tra Ready scene de ON MainCam OF LocalCam
            if (isReadyScene) {
                // (this.sceneToStart) networkPlayer <- spawner.cs <- dropdownscenename.cs
                /* if(Runner.IsSharedModeMasterClient) sceneToStart = spawner.gameMap.ToString(); */
                //Debug.Log("MAIN LOBBY SCENE NE");
                //Camera.main.transform.position = new Vector3(transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);

                // OF localCam
                localCameraHandler.gameObject.SetActive(false);
                localCameraHandler.localCamera.enabled = false;

                miniMapCameraHandler.gameObject.SetActive(false);
                miniMapCameraHandler.miniMapCamera.enabled = false;
                
                // OF localPlayer UI
                localUI.SetActive(false);

                // ON nickName if readyScene
                nickName_TM.gameObject.SetActive(true);
                miniMap_Image.SetActive(false);

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else {
                GetComponent<NetworkPlayer_Support>()?.Init();
                Debug.Log($"_____co chay NOT IsReadyScene");
                Utils.SetRenderLayerInChildren(playerModel, LayerMask.NameToLayer("LocalPlayerModel"));

                //? tat main camera cua local player
                if(Camera.main != null)
                    Camera.main.gameObject.SetActive(false);
                
                //? ON local camera
                localCameraHandler.localCamera.enabled = true;  // ON camera component
                localCameraHandler.gameObject.SetActive(true);  //ON ca gameObject LocalCameraHandler(co camera + gun)

                miniMapCameraHandler.miniMapCamera.enabled = true;
                miniMapCameraHandler.gameObject.SetActive(true);

                //? deAttach neu localCamera dang enable ra khoi folder cha
                localCameraHandler.transform.parent = null;
                miniMapCameraHandler.transform.parent = null;

                //? bat local UI | canvas cua ca local player(crossHair, onDamageImage, messages rpc send)
                localUI.SetActive(true); // con cua localCamera transform

                //? OFF nickName if KO dang o readyScene
                nickName_TM.gameObject.SetActive(false);
                miniMap_Image.SetActive(true);

                //? disable mouse de play
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = true;
            }

            // lay gia tri Gamemanager.playerNickName gan vao
            //RPC_SetNickName(GameManager.playerNickName);
            nickName = DataSaver.Instance.dataToSave.userName;
            if (nickName == "")
                nickName = "Guest";
            RPC_SetNickName(nickName);


            /* RPC_SetNickName(PlayerPrefs.GetString("PlayerNickName_Local")); */

            // kiem tra PlayerPref player (Object.InputAuthority.PlayerID) -> le = green | chan = red
            /* if(Object.InputAuthority.PlayerId % 2 != 0) isEnemy = false;
            else isEnemy = true;
            RPC_SetIsEnemyChanged(isEnemy); */

            //todo testing PlayerLeft
            Runner.SetPlayerObject(Object.InputAuthority, Object);


            // gan playerPref vao trong dictionary
            /* NetDict.Add(Object.InputAuthority.PlayerId, nickName_Network.ToString());
            RPC_SendNetDict(Object.InputAuthority.PlayerId, nickName_Network.ToString()); */

            //TODO KO HIEN THI NICKNAME O TAT CA CAC SCENE KHI DUOC SPAWN RA
            /* nickName_TM.gameObject.SetActive(false); */
        }
        else {
            localCameraHandler.localCamera.enabled = false;
            localCameraHandler.gameObject.SetActive(false);

            // tat miniMap camera
            miniMapCameraHandler.miniMapCamera.enabled = false;
            miniMapCameraHandler.gameObject.SetActive(false);
            miniMap_Image.SetActive(false); // ko show cho player clone

            localUI.SetActive(false);
        }

        //? set player as a player object -> khi player left se chi hien dung ten player roi
        Runner.SetPlayerObject(Object.InputAuthority, Object);

        /* Debug.Log($"_____Set ObjectNetwork = " + Object.GetComponent<NetworkPlayer>().nickName_Network.ToString()); */
        /* var name = GameManager.Instance.playerNickName; */

        // hien ten tren hierachy
        transform.name = $"P_{Object.Id} -> {nickName_Network.ToString()}";
    }

    //? gan nickName_Network cho bien texMeshPro GUI local
    private void OnNickNameChanged() {
        Debug.Log($"NickName changed to {nickName_Network} for player {gameObject.name}");
        nickName_TM.text = nickName_Network.ToString();

        if(!Object.HasStateAuthority) {
            //miniMap.SetActive(false);
        }
        
    }

    private void OnIsEnemyChanged() {
        /* if(spawner.customLobbyName != "OurLobbyID_Team") return; */

        if(isEnemy_Network) {
            nickName_TM.color = Color.red;
        } else nickName_TM.color = Color.green;
    }

    public void SetNicknameUIColor(Color color)
    {
        nickName_TM.color = color;
    }

    //? phuong thuc de local player send data cua rieng no len stateAuthority
    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetNickName(string nickName, RpcInfo info = default) {
        Debug.Log($"[RPC] Set nickName {nickName} for localPlayer");
        this.nickName_Network = nickName;

        //todo SEND TO ALL CLIENTS
        if(SceneManager.GetActiveScene().name == "Ready") return;
        StartCoroutine(SendPlayerNameJointToAllCO());
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void RPC_SetIsEnemyChanged(bool isEnemy, RpcInfo rpcInfo= default) {
        this.isEnemy_Network = isEnemy;
    }

    //? Add activePlayer into NetDict
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SendNetDict(int playerId, string nickName) {
        NetDict.Add(playerId, nickName);
    }

    IEnumerator SendPlayerNameJointToAllCO() {
        yield return new WaitForSeconds(0.5f);
        if(!isPublicJoinMessageSent) {
            networkInGameMessages.SendInGameRPCMessage(nickName_Network.ToString(), " -> Joined Room");
            isPublicJoinMessageSent = true;
        }
    }

    //? interface IPlayerLeft implement
    public void PlayerLeft(PlayerRef player) {
        // Who create room will send message playerLeft
        if(LocalDict.TryGetValue(player.PlayerId, out var value)) {
            networkInGameMessages.SendInGameRPCMessage(value.ToString(), " -> Left room");
        }

        if(player == Object.StateAuthority) {
            Runner.Despawn(Object);
            Debug.Log($"___NetworkPlayer Left Room");
        }
    }

    void OnDestroy() {
        // neu this.Object DeSpawn coll 240 - this.Object destroy - se destroy luon localCam cua no
        if(localCameraHandler != null) {
            Debug.Log("SU KIEN ONDESTROY LOCAL CAMERA HANDLER IN NETWORKPLAYER.CS");
            Destroy(localCameraHandler.gameObject);
        }
        if(miniMapCameraHandler != null) {
            Destroy(miniMapCameraHandler.gameObject);
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        Debug.Log($"{Time.time} OnSceneLoaded: " + scene.name);
        Debug.Log($"_____OnSceneLoaded() NetworkPlayer.cs");
        isPublicJoinMessageSent = false;

        if(scene.name != "MainLobby") {
            // thong bao cho host biet can phai Spawned code
            if(Object.HasStateAuthority && Object.HasInputAuthority) {
                Spawned();
            }
            if(Object.HasStateAuthority) {
                GetComponent<CharacterMovementHandler>().RequestRespawn();
            }

            //? load NetDic
            AddNetworkedDictionary();
        }
    }

    //? nut back main menu | leave trong game + leave ready scene
    public async void ShutdownLeftRoom() {
        await FindObjectOfType<NetworkRunner>().Shutdown();
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayerJoined(PlayerRef player) {
        Debug.Log($"_____playerJoint " + player.PlayerId);
    }

    void AddNetworkedDictionary() {
        foreach (var player in Runner.ActivePlayers)
        {
            PlayerRef playerRef = player;
            NetworkObject playerObject = Runner.GetPlayerObject(playerRef);
            if(playerObject != null && playerObject.TryGetComponent<NetworkPlayer>(out var nameComponent)) {
                Debug.Log($"playerID - {playerRef.PlayerId} | name - {nameComponent.nickName_Network}");
                NetDict.Add(playerRef.PlayerId, nameComponent.nickName_Network.ToString());
            }
        }

        LocalDict.Clear();
        foreach (var item in NetDict) {
            LocalDict.Add(item.Key, item.Value.ToString());
        }
    }
}