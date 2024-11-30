using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    [SerializeField] private string WALK_SLOW_ = "walk_slow_0";
    void Start()
    {
        audioSource.clip = SoundManager.SoundAsset.GetSound(WALK_SLOW_);
    }

    public void FootSound() 
    {
        audioSource.CustomPlaySound();
    }
    
}
