using Fusion;
using Photon.Voice.Unity;
using Photon.Voice.Fusion;
using UnityEngine;

public class VoiceChatManager : NetworkBehaviour
{
    // Reference to the Photon Voice Network component
    [SerializeField] private VoiceConnection voiceConnection;
    
    // Reference to the recorder that captures microphone input
    [SerializeField] private Recorder recorder;
    
    // Reference to the speaker that plays received audio
    [SerializeField] private Speaker speaker;

    private void Awake()
    {
        // Get or add required components
        voiceConnection = GetComponent<VoiceConnection>();
        if (voiceConnection == null)
        {
            //voiceConnection = gameObject.AddComponent<VoiceConnection>();
        }

        recorder = GetComponent<Recorder>();
        if (recorder == null)
        {
            //recorder = gameObject.AddComponent<Recorder>();
        }

        speaker = GetComponent<Speaker>();
        if (speaker == null)
        {
            //speaker = gameObject.AddComponent<Speaker>();
        }

        // Configure voice connection settings
        voiceConnection.PrimaryRecorder = recorder;
    }

    public override void Spawned()
    {
        // Only setup voice chat for the local player
        if (Object.HasInputAuthority)
        {
            SetupVoiceChat();
        }
    }

    private void SetupVoiceChat()
    {
        // Configure recorder settings
        recorder.TransmitEnabled = true;
        recorder.VoiceDetection = true;
        recorder.VoiceDetectionThreshold = 0.01f;
        recorder.InterestGroup = 0; // Use different groups for team-specific chat

        // Configure speaker settings
        speaker.PlayDelay = 200; // Adjust based on your needs
    }

    public void SetTeamChannel(int teamId)
    {
        // Set the interest group based on team ID
        if (Object.HasInputAuthority)
        {
            recorder.InterestGroup = (byte)teamId;
            //speaker.InterestGroup = (byte)teamId;
        }
    }

    // Toggle voice chat on/off
    public void ToggleVoiceChat(bool enabled)
    {
        if (Object.HasInputAuthority)
        {
            recorder.TransmitEnabled = enabled;
        }
    }

    // Toggle push-to-talk
    private void Update()
    {
        if (Object.HasInputAuthority)
        {
            if (Input.GetKeyDown(KeyCode.V)) // Change key as needed
            {
                recorder.TransmitEnabled = true;
                Debug.Log($"_____co nhan V");
            }
            if (Input.GetKeyUp(KeyCode.V))
            {
                recorder.TransmitEnabled = false;
            }
            //ToggleVoiceChat(recorder.TransmitEnabled );
        }
    }

}
