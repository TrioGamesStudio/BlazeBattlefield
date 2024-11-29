using UnityEngine;

public static class PlayerSoundHandler
{
    public static void CustomPlaySound(this AudioSource audioSource)
    {
        Debug.Log($"Audio Source is: {audioSource.name}",audioSource.gameObject);
        audioSource.Play();
    }
}