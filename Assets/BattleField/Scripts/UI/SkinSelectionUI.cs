using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class SkinSelectionUI : MonoBehaviour
{
    public GameObject skinSelectUIPrefab;
    public GameObject container;
    public List<Sprite> skinSpriteIcons = new();
    public List<SkinAvatarUI> avatarUIList = new();
    public SkinSelection skinSelection;
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
    }

    [Button]
    public void Init()
    {
        foreach(var sprite in skinSpriteIcons)
        {
            var icon = Instantiate(skinSelectUIPrefab, container.transform);
            icon.transform.GetChild(1).GetComponent<Image>().sprite = sprite;
        }
    }

   
}
