using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] string playSoundNam;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = SoundManager.Instance.scene1Sound;
        audioSource.Play();
    }

    
}
