using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundAsset", menuName = "SoundAsset")]
public class SoundAsset : ScriptableObject
{
    public static SoundAsset GetInstance => Resources.Load<SoundAsset>("SoundAsset");
    public List<SmallSoundAsset> _SoundAssets = new();

    private Dictionary<string, AudioClip> keyValuePairs = new();

    private void Convert()
    {
        foreach (var soundSmallAsset in _SoundAssets)
        {
            foreach (var item in soundSmallAsset._SoundAssets)
            {
                if (keyValuePairs.ContainsKey(item.soundName))
                {
                    return;
                }
                keyValuePairs.Add(item.soundName, item.AudioClip);
            }
        }
        
    }

    public AudioClip GetSound(string audioName)
    {
        if (string.IsNullOrEmpty(audioName))
        {
            Debug.Log("This audio name is null or empty");
        }
        if(keyValuePairs.Count == 0)
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
