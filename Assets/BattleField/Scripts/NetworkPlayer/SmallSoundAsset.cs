using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SmallSoundAsset", menuName = "SmallSoundAsset")]
public class SmallSoundAsset : ScriptableObject
{
    public List<_SoundAsset> _SoundAssets = new();
    public string prefix;
    public List<AudioClip> addClip = new();
    [Button]
    private void AddToList()
    {
        int index = 0;
        foreach(var item in addClip)
        {
            _SoundAsset _SoundAsset = new();
            _SoundAsset.soundName = prefix + index.ToString();
            _SoundAsset.AudioClip = item;
            _SoundAssets.Add(_SoundAsset);
            index++;
        }
        addClip.Clear();
    }
}