using System.Collections.Generic;
using NaughtyAttributes;

#if UNITY_EDITOR
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "Skin Data Creator", menuName = "Skin Data Creator")]
public class SkinDataCreator : ScriptableObject
{
    public List<Sprite> spritesIcons;
    [Button]
    public void CreateSkinData()
    {

    }
}