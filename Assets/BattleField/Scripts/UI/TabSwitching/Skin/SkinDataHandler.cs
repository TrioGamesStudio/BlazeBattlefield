using System.Collections.Generic;
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

    private void SkinLocker(bool isUnlock)
    {
        foreach (var skin in skinSpriteIcons)
        {
            skin.isUnlock = isUnlock;
        }
    }
}