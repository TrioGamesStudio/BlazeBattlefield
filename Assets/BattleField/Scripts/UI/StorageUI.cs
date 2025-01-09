using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StorageUI : MonoBehaviour
{
    public CanvasGroup storageCanvasGroup;
    public CanvasGroup mainLobbyCanvasGroup;

    public Button openStorageBtn;
    public Button openMainLobby;


    public UnityEvent onBlockShowStorageEvent;
    public UnityEvent onShowMainLobbyEvent;
    public UnityEvent onShowStorageEvent;

    [SerializeField] AnimatorLocalHandler animatorLocalHandler;
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
        bool cannotOpen = Matchmaking.Instance.currentMode == Matchmaking.Mode.Duo;

        if (cannotOpen)
        {
            onBlockShowStorageEvent?.Invoke();
            return;
        }

        ShowCanvasGroup(mainLobbyCanvasGroup, false);
        ShowCanvasGroup(storageCanvasGroup, true);
        onShowStorageEvent?.Invoke();

        if(DataSaver.Instance != null) {
            DataSaver.Instance.LoadData();
        }
        

        animatorLocalHandler.ActiveAnimatonLocal();
    }

    public void ShowMainLobby()
    {
        ShowCanvasGroup(mainLobbyCanvasGroup, true);
        ShowCanvasGroup(storageCanvasGroup, false);
        onShowMainLobbyEvent?.Invoke();

        animatorLocalHandler.DeActiveAnimatonLocal();
    }

    private void ShowCanvasGroup(CanvasGroup canvasGroup, bool enable)
    {
        float time = enable ? .4f : 0;
        float fade = enable ? 1 : 0;
        canvasGroup.DOFade(fade, time);
        canvasGroup.interactable = enable;
        canvasGroup.blocksRaycasts = enable;
    }
}
