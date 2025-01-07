using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundRequestManager : MonoBehaviour
{
    public static SoundRequestManager instance;

    [SerializeField] private AudioSource audioSourcePrefab;

    private void Awake()
    {
        instance = this;

        audioSourcePrefab.gameObject.SetActive(false);
    }
    public void PlayOneTime(AudioClip clip,Vector3 position)
    {
        var audioSource = Instantiate(audioSourcePrefab, position,Quaternion.identity);
        audioSource.gameObject.SetActive(true);
        audioSource.PlayOneShot(clip);
        Destroy(audioSource.gameObject, clip.length);
    }
}
