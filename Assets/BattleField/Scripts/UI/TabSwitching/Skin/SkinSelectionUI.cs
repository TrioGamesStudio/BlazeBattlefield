using DG.Tweening;
using NaughtyAttributes;
using System;
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

    public List<SkinAvatarUI> avatarUIList = new();
    [SerializeField] private SkinDataHandler SkinDataHandler;
    public SkinSelection skinSelection;

    public TextMeshProUGUI skinDescription;
    private void Start()
    {
        avatarUIList = GetComponentsInChildren<SkinAvatarUI>().ToList();
        foreach (var avatarUI in avatarUIList)
        {
            avatarUI.OnSkinSelection = OnClickUI;
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
        skinDescription.text = SkinDataHandler.skinSpriteIcons[index].skinDescription;
    }

    [Button]
    private void PreCreatingUIAvatar()
    {
        // use for editor'
        int index = 0;
        foreach (var skinData in SkinDataHandler.skinSpriteIcons)
        {
            var icon = (GameObject)PrefabUtility.InstantiatePrefab(skinSelectUIPrefab, container.transform);
            var skinAvatar = icon.GetComponent<SkinAvatarUI>();
            skinAvatar.avatarImg.sprite = skinData.avatarIcon;
            skinAvatar.skinIndex = index;
            index++;
            //icon.transform.GetChild(1).GetComponent<Image>().sprite = skinData.avatarIcon;
        }
    }

    private void RefreshUIByData()
    {
        for (int i = 0; i < avatarUIList.Count; i++)
        {
            var skinData = SkinDataHandler.skinSpriteIcons[i];
            var avatarUI = avatarUIList[i];

            avatarUI.SetPrice(skinData.price);
            avatarUI.ToggleLocker(skinData.isUnlock);
        }
    }
}
