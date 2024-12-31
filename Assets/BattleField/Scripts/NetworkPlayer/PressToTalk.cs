using System;
using System.Collections.Generic;
using Fusion;
using NaughtyAttributes;
using Photon.Voice.Fusion;
using Photon.Voice.Unity;
using UnityEngine;

public class PressToTalk : NetworkBehaviour, IPlayerJoined
{
    [SerializeField] VoiceConnection voiceConnection;
    [SerializeField] Recorder recorder;
    [SerializeField] Speaker speaker;

    public Dictionary<PlayerRef, int> playerTeams = new Dictionary<PlayerRef, int>();
    
    [Networked] public int TeamID{get; set;}
    [Networked] public NetworkBool IsTalking { get; set; }
    bool isGetActivePlayers = false;

    private void Awake() {
        recorder = FindObjectOfType<Recorder>();
        voiceConnection = FindObjectOfType<VoiceConnection>();
        speaker = GetComponentInChildren<Speaker>();
        recorder.TransmitEnabled = false;

    }
    public override void Spawned()
    {
        base.Spawned();
        // playerTeams[Object.InputAuthority] = TeamID;
        // Debug.Log($"_____" + playerTeams.Count);
    }


    private void Update() {
        //if(teamID <= 0) return;

        if(Input.GetKey(KeyCode.V)) {
            // EnableTalking();
            StartTeamVoice();
        }
        else if(Input.GetKeyUp(KeyCode.V)) {
            // DisableTalking();
            StopTeamVoice();
        }

        // lay danh sach khi bat dau tran
        if(!isGetActivePlayers && MatchmakingTeam.Instance.IsDone) {
            isGetActivePlayers = true;
            GetActivePlayers();
        }
    }

    void EnableTalking() {
        recorder.TransmitEnabled = true;
    }

    void DisableTalking() {
        recorder.TransmitEnabled = false;
    }

    public void SetTeamID(string id) {
        if(Object.HasStateAuthority) {
            RPC_RequestTeamID(id);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestTeamID(string id) {

        this.TeamID = Convert.ToInt32(id);
        /* recorder.InterestGroup = (byte)TeamID; */
    }

    private void StartTeamVoice()
    {
        if (!Object.HasInputAuthority) return;

        recorder.TransmitEnabled = true;
        IsTalking = true;

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
        if (!Object.HasInputAuthority) return;

        recorder.TransmitEnabled = false;
        IsTalking = false;

        // Reset voice transmission settings
        voiceConnection.Client.OpChangeGroups(
            null,
            new byte[] { (byte)TeamID }
        );
    }

    public void PlayerJoined(PlayerRef player)
    {
        /* playerTeams[player] = TeamID;
        Debug.Log($"_____list active players" + playerTeams.Count); */
    }

    [EditorButton]
    public void GetActivePlayers() {
        GameObject[] gameObjectsToTransfer = GameObject.FindGameObjectsWithTag("Player");
        foreach (var item in gameObjectsToTransfer)
        {
            playerTeams[item.GetComponent<NetworkObject>().InputAuthority] = item.GetComponent<PressToTalk>().TeamID;
            
        }
        Debug.Log($"_____list active players" + playerTeams.Count);
        
        foreach (var item in playerTeams)
        {
            Console.WriteLine($"____{item.Key} ----- {item.Value}");
        }
    }
}
