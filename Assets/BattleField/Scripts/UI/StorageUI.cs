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

        openStorageBtn.transform.parent = mainLobbyCanvasGroup.transform;

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
        mainLobbyCanvasGroup.DOFade(0, 0);
        mainLobbyCanvasGroup.interactable = false;
        storageCanvasGroup.DOFade(1, .4f);
        storageCanvasGroup.interactable = true;
    }

    public void ShowMainLobby()
    {
        storageCanvasGroup.DOFade(0, 0);
        storageCanvasGroup.interactable = false;
        mainLobbyCanvasGroup.DOFade(1, .4f);
        mainLobbyCanvasGroup.interactable = true;
    }
}
