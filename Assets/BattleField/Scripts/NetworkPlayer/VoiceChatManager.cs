using Fusion;
using Photon.Voice.Unity;
using Photon.Voice.Fusion;
using UnityEngine;
using System;

public class VoiceChatManager : NetworkBehaviour
{
    /* private VoiceConnection voiceConnection; */
    [SerializeField] private Recorder recorder;
    /* private Speaker speaker; */

    [Networked]
    private int TeamID { get; set; }

    private void Awake()
    {
        // Find the VoiceConnection in the scene
        /* voiceConnection = FindObjectOfType<VoiceConnection>();
        if (voiceConnection == null)
        {
            Debug.LogError("VoiceConnection not found! Make sure you have a VoiceConnection component in your scene.");
            return;
        } */
        /* if(recorder == null)
            recorder = GetComponent<Recorder>(); */

        SetupVoiceComponents();
    }

    private void SetupVoiceComponents()
    {
        // Add and configure recorder
        /* recorder = gameObject.GetComponent<Recorder>();
        if (recorder == null)
        {
            recorder = gameObject.AddComponent<Recorder>();
        } */

        // Important: Set the recorder as the primary recorder for the voice connection
        /* voiceConnection.PrimaryRecorder = recorder; */

        // Configure recorder settings
        recorder.VoiceDetection = true;
        recorder.VoiceDetectionThreshold = 0.01f;
        recorder.TransmitEnabled = false;

        // Add and configure speaker
        /* speaker = gameObject.GetComponent<Speaker>();
        if (speaker == null)
        {
            speaker = gameObject.AddComponent<Speaker>();
        } */

        Debug.Log("Voice components setup completed");
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            EnableVoice();
        }
    }

    private void EnableVoice()
    {
        if (recorder != null)
        {
            recorder.TransmitEnabled = true;
            Debug.Log("Voice transmission enabled for local player");
        }
    }

    public void SetTeamID(string teamID) {
        int id = Convert.ToInt32(teamID);
        RPC_SetTeamChannel(id);
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_SetTeamChannel(int teamId)
    {
        if (Object.HasInputAuthority && recorder != null)
        {
            TeamID = teamId;
            recorder.InterestGroup = (byte)teamId;
            Debug.Log($"Set voice chat to team channel: {teamId}");
        }
    }

    private void Update()
    {
        /* if (Object.HasInputAuthority && recorder != null)
        {
        } */
            // Push-to-talk with V key
            if (Input.GetKeyDown(KeyCode.V))
            {
                recorder.TransmitEnabled = true;
                Debug.Log("Voice transmission active");
            }
            if (Input.GetKeyUp(KeyCode.V))
            {
                recorder.TransmitEnabled = false;
                Debug.Log("Voice transmission stopped");
            }

            // Toggle mute with M key
            if (Input.GetKeyDown(KeyCode.M))
            {
                recorder.TransmitEnabled = !recorder.TransmitEnabled;
                Debug.Log($"Voice {(recorder.TransmitEnabled ? "unmuted" : "muted")}");
            }
    }
}