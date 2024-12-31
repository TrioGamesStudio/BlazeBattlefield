using System;
using Fusion;
using Photon.Voice.Fusion;
using Photon.Voice.Unity;
using UnityEngine;

public class PressToTalk : MonoBehaviour
{
    Recorder recorder;
    Speaker speaker;
    [SerializeField] VoiceConnection voiceConnection;
    int teamID = -1;

    private void Awake() {
        if(recorder == null) {
            recorder = GetComponent<Recorder>();
        }
            
        recorder.TransmitEnabled = false;

        /* byte[] groupsToSubscribe = { 1 };  // Example group ID to subscribe to
        byte[] groupsToUnsubscribe = null;

        if (voiceConnection != null)
        {
            voiceConnection.Client.OpChangeGroups(groupsToSubscribe, groupsToUnsubscribe);
            Debug.Log("Subscribed to group 1");
        } */
    }

    private void Update() {
        //if(teamID <= 0) return;

        if(Input.GetKey(KeyCode.V)) {
            EnableTalking();
        }
        else if(Input.GetKeyUp(KeyCode.V)) {
            DisableTalking();
        }
        
    }
    void EnableTalking() {
        recorder.TransmitEnabled = true;
    }

    void DisableTalking() {
        recorder.TransmitEnabled = false;
    }

    public void SetId(string id) {
        /* this.teamID = Convert.ToInt32(id) - 100;
        recorder.InterestGroup = (byte)teamID; */

        
    }

}
