using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnGame : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] string playSoundNam;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

}
