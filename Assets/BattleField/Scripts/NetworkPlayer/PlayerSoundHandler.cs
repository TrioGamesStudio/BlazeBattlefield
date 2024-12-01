using UnityEngine;

public static class PlayerSoundHandler
{
    public static void CustomPlaySound(this AudioSource audioSource, AudioClip audioClip = null)
    {
        if(audioClip == null)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.PlayOneShot(audioClip);
        }
        Debug.Log($"Audio Source is: {audioSource.name}",audioSource.gameObject);
    }
}