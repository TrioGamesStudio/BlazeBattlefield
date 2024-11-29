using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    AudioSource audioSource;
    // Start is called before the first frame update
    //private const string FOOD_STEP_SOUND_1 = "food_step_1";
    [SerializeField] private string food_step_1;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = SoundManager.soundAsset.GetSound(food_step_1);
    }

    public void FootSound() 
    {
        audioSource.CustomPlaySound();
    }
    
}
