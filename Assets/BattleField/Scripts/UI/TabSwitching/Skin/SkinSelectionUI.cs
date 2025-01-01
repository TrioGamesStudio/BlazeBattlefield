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
        avatarUIList = GetComponentsInChildren<SkinAvatarUI>().ToList();
        foreach (var avatarUI in avatarUIList)
        {
            avatarUI.OnSkinSelection = OnChangeSkinByIndex;
            avatarUI.OnUnlockSkin = TryToBuySkin;
            Debug.Log("On Assign Event", gameObject);
        }
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
        int currentCoint = DataSaver.Instance.dataToSave.coins;
        int skinPrice = SkinDataHandler.skinSpriteIcons[buyIndex].price;
        return currentCoint >= skinPrice;
    }

    private void OnChangeSkinByIndex(int index)
    {
        Debug.Log("On Click UI:" + index, gameObject);

        foreach (var avatar in avatarUIList)
        {
            avatar.button.interactable = true;
            avatar.DeSelect();
        }
        OnChangedSkinAction?.Invoke(index);
        avatarUIList[index].button.interactable = false;
        avatarUIList[index].Select();
        skinDescription.text = SkinDataHandler.skinSpriteIcons[index].skinDescription;
        BuyPanel.gameObject.SetActive(false);
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
    public void RefreshUIByData()
    {
        SkinDataHandler.UnlockPlayerOwnSkin(DataSaver.Instance.inventoryDataToSave.skinsLists);

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
        DataSaver.Instance.inventoryDataToSave.skinsLists = SkinDataHandler.GetAllUnlockSkin();
        DataSaver.Instance.SaveInventoryData();
    }

    public void SaveDefaultSkin()
    {
        DataSaver.Instance.inventoryDataToSave.skinsLists = SkinDataHandler.GetDefautlSkinData();
        DataSaver.Instance.SaveInventoryData();
    }
}
