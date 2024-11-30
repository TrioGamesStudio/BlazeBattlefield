using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundReady : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] private string GO_ = "go_3";
    bool isPlayReady = false;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = SoundManager.SoundAsset.GetSound(GO_);
    }

    private void Update() {
        if(Matchmaking.Instance.IsDone && !isPlayReady) {
            isPlayReady = true;
            audioSource.CustomPlaySound();
        }
    }

}
