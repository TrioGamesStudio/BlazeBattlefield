using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinAvatarUI : MonoBehaviour
{
    public Action<int> OnSkinSelection;
    public Action<int> OnUnlockSkin;
    public Button button;
    public Image avatarImg;
    [Header("Frame_Edge")]
    [SerializeField] private GameObject innerFrame;
    [SerializeField] private GameObject selectFrame;
    [Header("Locker")]
    [SerializeField] private GameObject lockUI;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Sprite lockSprite;
    [SerializeField] private Sprite unlockSprite;
    [SerializeField] private Image lockerIcon;
    public int skinIndex;
    public bool isUnlock;
    private void Awake()
    {
        button.onClick.AddListener(RaiseEvent);
        DeSelect();
    }
    private void OnDestroy()
    {
        button.onClick.RemoveListener(RaiseEvent);
    }

    private void RaiseEvent()
    {
        if (isUnlock == false)
        {
            OnUnlockSkin?.Invoke(skinIndex);
            return;
        }

        OnSkinSelection?.Invoke(skinIndex);
    }

    public void DeSelect()
    {
        selectFrame.SetActive(false);
        innerFrame.SetActive(false);
    }

    public void Select()
    {
        selectFrame.SetActive(true);
        innerFrame.SetActive(true);
    }

    public void SetPrice(int price)
    {
        priceText.text = price.ToString();
    }

    public void Lock() => lockUI.gameObject.SetActive(true);
    public void Unlock() => lockUI.gameObject.SetActive(false);

    public void TryToUnlock(bool isTryToUnlock)
    {
        if (isTryToUnlock)
        {
            lockerIcon.DOKill();
            lockerIcon.sprite = unlockSprite;
        }
        else
        {
            lockerIcon.sprite = lockSprite;
        }
    }

    public void ToggleLocker(bool isUnlock)
    {
        this.isUnlock = isUnlock;
        if (isUnlock)
        {
            Unlock();
        }
        else
        {
            Lock();
        }
    }
}
