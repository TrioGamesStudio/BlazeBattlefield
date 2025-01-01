using System.Collections.Generic;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "Skin Data Handler", menuName = "Skin Data Handler")]
public class SkinDataHandler : ScriptableObject
{
    public List<SkinData> skinSpriteIcons = new();
    public List<string> GetDefautlSkinData()
    {
        {
            List<string> defaultSkins = new List<string>();

            for (int i = 0; i < 3 && i < skinSpriteIcons.Count; i++)
            {
                defaultSkins.Add(skinSpriteIcons[i].skinName);
            }

            return defaultSkins;
        }
    }
    public List<string> GetAllUnlockSkin()
    {
        List<string> unlockSkin = new List<string>();
        foreach (var skin in skinSpriteIcons)
        {
            if (skin.isUnlock)
                unlockSkin.Add(skin.skinName);
        }
        return unlockSkin;
    }
    public void UnlockPlayerOwnSkin(List<string> skinOwns)
    {
        foreach (var skinIngame in skinSpriteIcons)
        {
            if (skinOwns.Contains(skinIngame.skinName))
            {
                skinIngame.isUnlock = true;
            }
            else
            {
                skinIngame.isUnlock = false;
            }
        }
    }

    [Button]
    public void UnlockAll()
    {
        SkinLocker(true);
    }
    [Button]
    public void LockAll()
    {
        SkinLocker(false);
    }
    private void SkinLocker(bool isUnlock)
    {
        foreach (var skin in skinSpriteIcons)
        {
            skin.isUnlock = isUnlock;
        }
    }
    [Button]
    private void CreatePriceForSkin()
    {
        int count = 0;
        int price = 100;
        foreach (var skin in skinSpriteIcons)
        {
            if (count == 3)
            {
                price += 100;
                count = 0;
            }
            skin.price = price;
            count++;
#if UNITY_EDITOR
            EditorUtility.SetDirty(skin);
#endif
        }
       
    }
}