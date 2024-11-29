using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class  SoundManager 
{
    public static SoundAsset _soundAsset;
    public static SoundAsset SoundAsset
    {
        get
        {
            if(_soundAsset == null)
            {
                _soundAsset = SoundAsset.GetInstance;
            }
            return _soundAsset;
        }
    }
}
