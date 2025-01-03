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
    }
    private void OnDestroy()
    {
        SkinSelectionUI.OnChangedSkinAction -= SkinSelectionUI_OnChangedSkinAction;
    }
    private void SkinSelectionUI_OnChangedSkinAction(int newHatIndex)
    {
        HatDataHandler.currentHatIndex = newHatIndex;
        characterHatHandler.CreateHatLocal();
    }
}