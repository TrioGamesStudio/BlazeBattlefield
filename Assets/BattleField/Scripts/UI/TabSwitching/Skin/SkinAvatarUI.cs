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
    private bool isSelect = false;
    public void TryToUnlock(bool isTryToUnlock)
    {
        lockerIcon.DOKill();
        if (isTryToUnlock)
        {
            lockerIcon.sprite = unlockSprite;
            lockerIcon.transform.DOScale(Vector3.one * 1.3f,.25f).SetEase(Ease.InOutBounce);
            isSelect = true;
        }
        else
        {
            lockerIcon.sprite = lockSprite;
            if (isSelect)
            {
                lockerIcon.transform.DOScale(Vector3.one , .1f);
                isSelect = false;
            }
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
