using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundBackGround : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] string BACKGROUND_MUSIC_INDEX = "background_music_";

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = SoundManager.SoundAsset.GetSound(BACKGROUND_MUSIC_INDEX);
        audioSource.CustomPlaySound();
    }

}
