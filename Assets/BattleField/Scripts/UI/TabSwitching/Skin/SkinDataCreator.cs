#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor;
[CreateAssetMenu(fileName = "Skin Data Creator", menuName = "Skin Data Creator")]
public class SkinDataCreator : ScriptableObject
{
    public List<Sprite> spritesIcons;
    [Button]

    public void CreateMultipleSkin()
    {
        foreach (var skin in spritesIcons)
        {
            CreateSkinData(skin);
        }
    }
    public void CreateSkinData(Sprite sprite)
    {
        // Create a new instance of SkinData
        SkinData newSkinData = ScriptableObject.CreateInstance<SkinData>();

        // Set default values for the new SkinData
        newSkinData.skinName = sprite.name;
        newSkinData.skinDescription = "Description of the new skin";
        newSkinData.avatarIcon = sprite;
        newSkinData.price = 0;
        newSkinData.isUnlock = false;

        // Save the new SkinData asset
        string path = $"Assets/BattleField/ScriptableObject/HatData/{newSkinData.skinName}.asset";
        AssetDatabase.CreateAsset(newSkinData, path);
        AssetDatabase.SaveAssets();

        // Focus on the newly created asset
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newSkinData;
    }
}
#endif