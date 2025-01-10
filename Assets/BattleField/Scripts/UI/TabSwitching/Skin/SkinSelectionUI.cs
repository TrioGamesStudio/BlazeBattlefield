using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
public class SkinSelectionUI : MonoBehaviour
{
    [SerializeField] private GameObject skinSelectUIPrefab;
    [SerializeField] private GameObject container;

    [SerializeField] private List<SkinAvatarUI> avatarUIList = new();
    [SerializeField] private SkinDataHandler SkinDataHandler;

    [SerializeField] private TextMeshProUGUI skinDescription;
    [Header("Buy Panel")]
    [SerializeField] private TextMeshProUGUI buyStatusText;
    [SerializeField] private GameObject BuyPanel;
    [SerializeField] private Button buyBtn;
    [SerializeField] private float hideBuyPanelTime = .2f;
    [SerializeField] private ShowPlayerInfo showPlayerInfo;
    [SerializeField] private Color priceColor;
    [SerializeField] private Image buyPanelSkinIcon;
    [Header("Audio")]
    [SerializeField] private AudioSource selectAudio;
    [SerializeField] private AudioSource accpetAudio;
    [SerializeField] private AudioSource cancelAudio;
    [SerializeField] private AudioSource selectLockSkinAudio;

    
    public event Action<int> OnChangedSkinAction;
    private int buyIndex;

    private void Awake()
    {
        buyBtn.onClick.AddListener(Buy);
    }

    private void OnDestroy()
    {
        buyBtn.onClick.RemoveListener(Buy);
    }

    private void Start()
    {
        BuyPanel.gameObject.SetActive(false);
        foreach (var avatarUI in avatarUIList)
        {
            avatarUI.OnSkinSelection = OnChangeSkinByIndex;
            avatarUI.OnUnlockSkin = TryToBuySkin;
        }
        SetupSound();
    }

    [Button]
    private void SetupSound()
    {
        selectAudio.clip = SoundManager.SoundAsset.GetSound("select_skin");
        accpetAudio.clip = SoundManager.SoundAsset.GetSound("skin_selection_buy");
        cancelAudio.clip = SoundManager.SoundAsset.GetSound("skin_selection_cancel");
        selectLockSkinAudio.clip = SoundManager.SoundAsset.GetSound("select_lock_avatar");
    }

    private void TryToBuySkin(int skinIndex)
    {
        int price = SkinDataHandler.skinSpriteIcons[skinIndex].price;

        BuyPanel.SetActive(true);
        buyIndex = skinIndex;
        buyPanelSkinIcon.sprite = SkinDataHandler.skinSpriteIcons[skinIndex].avatarIcon;

        buyStatusText.text = $"Are you sure to buy this skin with price <color=#{ColorUtility.ToHtmlStringRGB(priceColor)}>{price}</color>";
        // reset state for UI
        buyBtn.interactable = CanBuySkin();
        SetChooseSkinToBuy(skinIndex);

        selectLockSkinAudio.Play();
    }

    private void SetChooseSkinToBuy(int skinIndex)
    {
        foreach (var item in avatarUIList)
        {
            if (item.skinIndex == skinIndex)
            {
                item.TryToUnlock(true);
            }
            else
            {
                item.TryToUnlock(false);
            }
        }
    }

    private void Buy()
    {
        if (CanBuySkin())
        {
            SkinDataHandler.skinSpriteIcons[buyIndex].isUnlock = true;
            SaveSkin();
            RefreshUIByData();
            showPlayerInfo.ChangeCoin(-SkinDataHandler.skinSpriteIcons[buyIndex].price);
            StartCoroutine(HideBuyPanel());
        }
    }

    private IEnumerator HideBuyPanel()
    {
        yield return new WaitForSeconds(hideBuyPanelTime);
        BuyPanel.gameObject.SetActive(false);
    }

    private bool CanBuySkin()
    {
        int currentCoint = 0;
        int skinPrice = 0;

        if (DataSaver.Instance != null) {

#if UNITY_WEBGL
            currentCoint = DataSaver.Instance.dataToSave.coins;

#else
            currentCoint = DataSaver.Instance.dataToSave.coins;

#endif

            skinPrice = SkinDataHandler.skinSpriteIcons[buyIndex].price;
        }
        
        return currentCoint >= skinPrice;
    }

    private void OnChangeSkinByIndex(int index)
    {
        Debug.Log("On Click UI:" + index, gameObject);

        ResetAvatarButtonState();

        OnChangedSkinAction?.Invoke(index);
       
        SetSelectButton(index);

        skinDescription.text = SkinDataHandler.skinSpriteIcons[index].skinDescription;

        BuyPanel.gameObject.SetActive(false);

        selectAudio.Play();
    }

    public void SetDeaultSkin(int defaultIndex)
    {
        // duplicate code !!!!
        ResetAvatarButtonState();

        OnChangedSkinAction?.Invoke(defaultIndex);

        SetSelectButton(defaultIndex);

        skinDescription.text = SkinDataHandler.skinSpriteIcons[defaultIndex].skinDescription;

        BuyPanel.gameObject.SetActive(false);

    }

    private void SetSelectButton(int index)
    {
        avatarUIList[index].button.interactable = false;
        avatarUIList[index].Select();
    }

    private void ResetAvatarButtonState()
    {
        foreach (var avatar in avatarUIList)
        {
            avatar.button.interactable = true;
            avatar.DeSelect();
        }
    }

#if UNITY_EDITOR
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
#endif
    [Button]
    public void RefreshUIByData()
    {
        if(DataSaver.Instance != null)
        {
            SkinDataHandler.UnlockPlayerOwnSkin(DataSaver.Instance.inventoryDataToSave.GetCollections(SkinDataHandler.CollectionsName));
        }

        for (int i = 0; i < avatarUIList.Count; i++)
        {
            var skinData = SkinDataHandler.skinSpriteIcons[i];
            var avatarUI = avatarUIList[i];

            avatarUI.SetPrice(skinData.price);
            avatarUI.ToggleLocker(skinData.isUnlock);
        }
    }


    [Button]
    public void SaveSkin()
    {
        if(DataSaver.Instance == null) return;
        DataSaver.Instance.inventoryDataToSave.SaveSkinData(SkinDataHandler.CollectionsName,SkinDataHandler.GetAllUnlockSkin());
        DataSaver.Instance.SaveInventoryData();
    }
    [Button]
    public void SaveDefaultSkin()
    {
        if(DataSaver.Instance == null) return;
        DataSaver.Instance.inventoryDataToSave.SaveSkinData(SkinDataHandler.CollectionsName,SkinDataHandler.GetDefautlSkinData());
        DataSaver.Instance.SaveInventoryData();
    }


    
}
