using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageUI : MonoBehaviour
{
    public CanvasGroup storageCanvasGroup;
    public CanvasGroup mainLobbyCanvasGroup;

    public Button openStorageBtn;
    public Button openMainLobby;
    private void Awake()
    {
        ShowMainLobby();
        openStorageBtn.onClick.AddListener(ShowStorage);
        openMainLobby.onClick.AddListener(ShowMainLobby);
    }

    private void OnDestroy()
    {
        openStorageBtn.onClick.RemoveListener(ShowStorage);
        openMainLobby.onClick.RemoveListener(ShowMainLobby);
    }

    public void ShowStorage()
    {
        ShowCanvasGroup(mainLobbyCanvasGroup, false);
        ShowCanvasGroup(storageCanvasGroup, true);
        //mainLobbyCanvasGroup.DOFade(0, 0);
        //mainLobbyCanvasGroup.interactable = false;
        //mainLobbyCanvasGroup.blocksRaycasts = false;
        //storageCanvasGroup.DOFade(1, .4f);
        //storageCanvasGroup.interactable = true;
        //storageCanvasGroup.blocksRaycasts = true;
    }

    public void ShowMainLobby()
    {
        ShowCanvasGroup(mainLobbyCanvasGroup, true);
        ShowCanvasGroup(storageCanvasGroup, false);
        //storageCanvasGroup.DOFade(0, 0);
        //storageCanvasGroup.interactable = false;
        //mainLobbyCanvasGroup.DOFade(1, .4f);
        //mainLobbyCanvasGroup.interactable = true;
    }

    private void ShowCanvasGroup(CanvasGroup canvasGroup,bool enable)
    {
        float time = enable ? .4f : 0;
        float fade = enable ? 1 : 0;
        canvasGroup.DOFade(fade, time);
        canvasGroup.interactable = enable;
        canvasGroup.blocksRaycasts = enable;
    }
}
