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
    public CanvasGroup mainLobbyCanvas;
    public CanvasGroup view;
    public List<SkinAvatarUI> avatarUIList = new();
    public SkinSelection skinSelection;

    private void Awake()
    {
        Hide();
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

    public void Hide()
    {
        view.DOFade(0, 0);
        view.interactable = false;
        mainLobbyCanvas.DOFade(1, .4f);
        mainLobbyCanvas.interactable = true;
    }
    public void Show()
    {
        view.DOFade(1, .4f);
        view.interactable = true;
        mainLobbyCanvas.DOFade(0,0);
        mainLobbyCanvas.interactable = false;   
    }
}
