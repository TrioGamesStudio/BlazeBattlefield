using Photon.Voice.Unity;
using UnityEngine;

public class PressToTalk : MonoBehaviour
{
    Recorder recorder;

    private void Awake() {
        if(recorder == null)
            recorder = GetComponent<Recorder>();
    }

    private void Update() {

        if(Input.GetKey(KeyCode.T)) {
            EnableTalking();
        }
        else if(Input.GetKeyUp(KeyCode.T)) {
            DisableTalking();
        }

    }
    void EnableTalking() {
        recorder.TransmitEnabled = true;
    }

    void DisableTalking() {
        recorder.TransmitEnabled = false;
    }

}
