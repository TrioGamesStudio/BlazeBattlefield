using DG.Tweening;
using NaughtyAttributes;
using System;
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
    private void Start()
    {
        avatarUIList = GetComponentsInChildren<SkinAvatarUI>().ToList();
        foreach (var avatarUI in avatarUIList)
        {
            avatarUI.OnClickUI = OnClickUI;
            Debug.Log("On Assign Event", gameObject);
        }
    }

    private void OnClickUI(int index)
    {
        Debug.Log("On Click UI:" + index, gameObject);

        foreach (var avatar in avatarUIList)
        {
            avatar.button.interactable = true;
            avatar.DeSelect();
        }
        skinSelection.SetSkinByIndex(index);
        avatarUIList[index].button.interactable = false;
        avatarUIList[index].Select();
        skinDescription.text = skinSpriteIcons[index].skinDescription;
    }

    [Button]
    private void PreCreatingUIAvatar()
    {
        // use for editor
        foreach (var skinData in skinSpriteIcons)
        {
            var icon = (GameObject)PrefabUtility.InstantiatePrefab(skinSelectUIPrefab, container.transform);
            icon.GetComponent<SkinAvatarUI>().avatarImg.sprite = skinData.avatarIcon;
            //icon.transform.GetChild(1).GetComponent<Image>().sprite = skinData.avatarIcon;
        }
    }
}
