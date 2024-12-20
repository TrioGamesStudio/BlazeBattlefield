using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public class SkinSelectionUI : MonoBehaviour
{
    public GameObject skinSelectUIPrefab;
    public GameObject container;

    public List<SkinData> skinSpriteIcons = new();
    public List<SkinAvatarUI> avatarUIList = new();
    
    public SkinSelection skinSelection;

    public TextMeshProUGUI skinDescription;

    private void Awake()
    {
        avatarUIList = GetComponentsInChildren<SkinAvatarUI>().ToList();
        foreach (var avatarUI in avatarUIList)
        {
            avatarUI.OnClickUI = OnClickUI;
        }
 
    }

    private void OnClickUI(int index)
    {
        foreach(var avatar in avatarUIList)
        {
            avatar.button.interactable = true;
        }
        skinSelection.SetSkinByIndex(index);
        avatarUIList[index].button.interactable = false;

        skinDescription.text = skinSpriteIcons[index].skinDescription;
    }

    [Button]
    private void PreCreatingUIAvatar()
    {
        // use for editor
        foreach(var skinData in skinSpriteIcons)
        {
            var icon = (GameObject)PrefabUtility.InstantiatePrefab(skinSelectUIPrefab, container.transform);
            icon.GetComponent<SkinAvatarUI>().avatarImg.sprite = skinData.avatarIcon;
            //icon.transform.GetChild(1).GetComponent<Image>().sprite = skinData.avatarIcon;
        }
    }
}
