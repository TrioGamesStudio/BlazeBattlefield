using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SmallSoundAsset", menuName = "SmallSoundAsset")]
public class SmallSoundAsset : ScriptableObject
{
    public List<_SoundAsset> _SoundAssets = new();
}