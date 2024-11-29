using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class  SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Player Sound")]
    public AudioClip footStepSound;

    [Header("BackGround Sound")]
    public AudioClip scene1Sound;
    public AudioClip scene2Sound;


    private void Awake() {
        DontDestroyOnLoad(this);
    }

    
}
