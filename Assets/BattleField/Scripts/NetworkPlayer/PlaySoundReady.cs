using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundReady : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] private string GET_READY_ = "get_ready_1";
    bool isPlayReady = false;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = SoundManager.SoundAsset.GetSound(GET_READY_);
    }

    private void Update() {
        if(Matchmaking.Instance.IsDone && !isPlayReady) {
            isPlayReady = true;
            audioSource.CustomPlaySound();
        }
    }

}
