using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using NaughtyAttributes;
using Photon.Voice.Fusion;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PressToTalk : NetworkBehaviour, IPlayerJoined
{
    [SerializeField] VoiceConnection voiceConnection;
    [SerializeField] Recorder recorder;
    [SerializeField] Speaker speaker;

    public Dictionary<PlayerRef, int> playerTeams = new Dictionary<PlayerRef, int>();
    
    [Networked] public int TeamID{get; set;}
    [Networked] public NetworkBool IsTalking { get; set; }
    [SerializeField] bool isTalking = false;
    [SerializeField] bool isGetActivePlayers = false;
    [SerializeField] bool isActiveChatVoice = false;
    [SerializeField] int playersInRoom = 0;
    public int PlayersInRoom{get { return playersInRoom;}}
    CharacterInputHandler characterInputHandler;

    bool isPressChatVoiceButton = false;
    ChatVoiceState chatVoiceState;
    PlayerRoomController playerRoomController;
    private void Awake() {
        characterInputHandler = GetComponent<CharacterInputHandler>();
        chatVoiceState = FindObjectOfType<ChatVoiceState>();
        recorder = FindObjectOfType<Recorder>();
        voiceConnection = FindObjectOfType<VoiceConnection>();
        speaker = GetComponentInChildren<Speaker>();
        recorder.TransmitEnabled = false;
        isGetActivePlayers = false;
        isActiveChatVoice = false;
        isTalking = false;
        isPressChatVoiceButton = false;
    }

    private void Update() {

        if(Object.HasStateAuthority == false) return;

        // lay danh sach khi bat dau tran Dou
        if(!isGetActivePlayers && MatchmakingTeam.Instance.IsDone) {
            isGetActivePlayers = true;
            GetActivePlayers();
        }

        // lay danh sach trong main lobby Dou
        if(SceneManager.GetActiveScene().name == "MainLobby") {
            if(!isGetActivePlayers && playersInRoom >= 2) {
                isGetActivePlayers = true;
                GetActivePlayers();
            }
        }

        if(Object.HasStateAuthority) {
            if(recorder.InterestGroup == 0) return; // solo mode ko chat voice

            if(characterInputHandler.IsChatVoice && !isPressChatVoiceButton) {
                isPressChatVoiceButton = true;
                if(!isActiveChatVoice) {
                    StartCoroutine(ActiveChatVoiceCO());
                    isActiveChatVoice = true;
                    return;
                }
                StartTeamVoice();
            } else if(!characterInputHandler.IsChatVoice && isPressChatVoiceButton){
                isPressChatVoiceButton = false;
                StopTeamVoice();
            }
        }
        PrintList();
    }

    public void SetTeamID(string id) {
        if(Object.HasStateAuthority) {
            RPC_RequestTeamID(id);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestTeamID(string id) {
        this.TeamID = Convert.ToInt32(id);
        recorder.InterestGroup = (byte)TeamID;
    }

    private void StartTeamVoice()
    {
        //if (!Object.HasInputAuthority) return;

        recorder.TransmitEnabled = true;
        IsTalking = true;
        isTalking = true;
        // Only transmit to players on the same team
        foreach (var player in playerTeams)
        {
            if (player.Value == TeamID)
            {
                // Enable voice transmission to this player
                voiceConnection.Client.OpChangeGroups(
                    new byte[] { (byte)TeamID }, // Interest groups to subscribe to
                    null  // Interest groups to unsubscribe from
                );
            }
        }
    }
    private void StopTeamVoice()
    {
        //if (!Object.HasInputAuthority) return;

        recorder.TransmitEnabled = false;
        IsTalking = false;
        isTalking = false;
        // Reset voice transmission settings
        voiceConnection.Client.OpChangeGroups(
            null,
            new byte[] { (byte)TeamID }
        );
    }

    [EditorButton]
    public void GetActivePlayers() {
        GameObject[] gameObjectsToTransfer = GameObject.FindGameObjectsWithTag("Player");
        foreach (var item in gameObjectsToTransfer)
        {
            playerTeams[item.GetComponent<NetworkObject>().InputAuthority] = item.GetComponent<PressToTalk>().TeamID;
            
        }
        Debug.Log($"_____list active players" + playerTeams.Count);
        PrintList();
    }

    void PrintList() {
        foreach (var item in playerTeams)
        {
            Debug.Log($"____{item.Key} ----- {item.Value}");
        }
    }

    IEnumerator ActiveChatVoiceCO() {
        chatVoiceState.AvtiveChatVoiceButton();
        StartTeamVoice();
        yield return new WaitForSeconds(0.2f);
        StopTeamVoice();
    }

    public void PlayerJoined(PlayerRef player)
    {
        playersInRoom += 1;
    }
}
