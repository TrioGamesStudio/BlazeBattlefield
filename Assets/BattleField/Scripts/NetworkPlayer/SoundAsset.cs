using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="asd",menuName ="asd")]
public class SoundAsset : ScriptableObject
{


    public List<_SoundAsset> _SoundAssets = new();
    private Dictionary<string, AudioClip> keyValuePairs;

    private void Convert()
    {
        foreach(var item in _SoundAssets)
        {
            if (keyValuePairs.ContainsKey(item.soundName))
            {
                return;
            }
            keyValuePairs.Add(item.soundName, item.AudioClip);
        }
    }

    public AudioClip GetSound(string audioName)
    {
        if (string.IsNullOrEmpty(audioName))
        {
            Debug.Log("This audio name is null or empty");
        }
        if(keyValuePairs == null)
        {
            Convert();
        }
        if(keyValuePairs.TryGetValue(audioName,out var audioClip))
        {
            return audioClip;
        }
        return null;
    }
}
[Serializable]
public class _SoundAsset
{
    public string soundName;
    public AudioClip AudioClip;
}