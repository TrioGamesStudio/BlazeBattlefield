using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioTest : MonoBehaviour
{
    public UnityEvent unityEvent;
    public AudioSource audioSource;
    public string id;

    [Button]
    private void GetSound()
    {
        audioSource.clip = SoundManager.SoundAsset.GetSound(id);
    }
    [Button]
    private void Test()
    {
        unityEvent?.Invoke();
    }
}
