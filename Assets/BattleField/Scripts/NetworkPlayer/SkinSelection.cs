using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinSelection : MonoBehaviour
{
    public static SkinSelection Instance;

    [SerializeField] Matchmaking matchmaking;
    [SerializeField] MatchmakingTeam matchmakingTeam;

    //[SerializeField] int skinsNextNumber = 0;
    [SerializeField] Transform skinsParent;
    int skinMaxNumber;
    // buttons
    [SerializeField] Button selectButton;
    [SerializeField] private SkinSelectionUI skinSelectionUI;
    [SerializeField] private SkinDataHandler skinDataHandler;

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    
        matchmaking = FindObjectOfType<Matchmaking>();
        matchmakingTeam = FindObjectOfType<MatchmakingTeam>();
        skinMaxNumber = skinsParent.childCount;

        skinSelectionUI.OnChangedSkinAction += SetSkinByIndex;
       
        SettingPanel.OnLogoutEvent += ResetSkinIndex;
    }

    private void OnDestroy()
    {
        skinSelectionUI.OnChangedSkinAction -= SetSkinByIndex;
        SettingPanel.OnLogoutEvent -= ResetSkinIndex;
    }

    private void Start()
    {
        if (Matchmaking.Instance.currentMode == Matchmaking.Mode.Solo)
        {
            int currentSkin = skinDataHandler.CurrentSkinIndex;

            SkinsSlectionSoloUpdate(currentSkin);

            matchmaking.SkinSelectedNumber = currentSkin;
            matchmakingTeam.SkinSelectedNumber = currentSkin;
        }

    }

    

    void SkinsSlectionSoloUpdate(int skinNumber)
    {
        foreach (Transform item in skinsParent)
        {
            item.gameObject.SetActive(false);
        }

        skinsParent.GetChild(skinNumber).gameObject.SetActive(true);
    }

    public void ToggleSelectSkinButton(bool isActive)
    {
        //selectButton.gameObject.SetActive(isActive);
    }

    public void SetSkinByIndex(int newIndex)
    {
        int skinsNextNumber = newIndex;
        skinDataHandler.CurrentSkinIndex = newIndex;

        SkinsSlectionSoloUpdate(skinsNextNumber);

        matchmaking.SkinSelectedNumber = skinsNextNumber;
        matchmakingTeam.SkinSelectedNumber = skinsNextNumber;
    }

    private void OnApplicationQuit()
    {
        ResetSkinIndex();
    }

    public void ResetSkinIndex()
    {
        skinDataHandler.CurrentSkinIndex = 0;
        skinDataHandler.LockAll();
    }
}
