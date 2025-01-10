#if UNITY_EDITOR
#endif
using UnityEngine;

public class HatSelection : MonoBehaviour
{
    public static HatSelection Instance;
    [SerializeField] private SkinSelectionUI SkinSelectionUI;
    [SerializeField] private HatDataHandler HatDataHandler;
    [SerializeField] private CharacterHatHandler characterHatHandler;
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

        SkinSelectionUI.OnChangedSkinAction += SkinSelectionUI_OnChangedSkinAction;
        SkinSelectionUI.SetDeaultSkin(HatDataHandler.currentHatIndex);
        SettingPanel.OnLogoutEvent += ResetHatIndex;
    }
    private void OnDestroy()
    {
        SkinSelectionUI.OnChangedSkinAction -= SkinSelectionUI_OnChangedSkinAction;
        SettingPanel.OnLogoutEvent -= ResetHatIndex;

    }
    private void SkinSelectionUI_OnChangedSkinAction(int newHatIndex)
    {
        HatDataHandler.currentHatIndex = newHatIndex;
        characterHatHandler.CreateHatLocal();
    }

    private void OnApplicationQuit()
    {
        ResetHatIndex();
    }

    public void ResetHatIndex()
    {
        HatDataHandler.currentHatIndex = 0;
        HatDataHandler.LockAll();
        SkinSelectionUI.SetDeaultSkin(HatDataHandler.currentHatIndex);

    }
}