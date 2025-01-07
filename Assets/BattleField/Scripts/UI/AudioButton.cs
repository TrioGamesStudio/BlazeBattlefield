using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Button))]
public class AudioButton : MonoBehaviour
{
    public string audioName;
    private Button btn;
    private AudioSource audioSource;
    private void Awake()
    {
        btn = GetComponent<Button>();
        audioSource = GetComponent<AudioSource>();
        btn.onClick.AddListener(PlaySound);
        audioSource.clip = SoundManager.SoundAsset.GetSound(audioName);
    }

    private void OnDestroy()
    {
        btn.onClick.RemoveListener(PlaySound);
    }

    private void PlaySound()
    {
        audioSource.Play();
    }
}
