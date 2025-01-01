#if UNITY_EDITOR
#endif
using UnityEngine;

public class HatSelection : MonoBehaviour
{
    public static HatSelection Instance;
    public SkinSelectionUI SkinSelectionUI;
    public HatDataHandler HatDataHandler;
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
    }
    private void OnDestroy()
    {
        SkinSelectionUI.OnChangedSkinAction -= SkinSelectionUI_OnChangedSkinAction;
    }
    private void SkinSelectionUI_OnChangedSkinAction(int newHatIndex)
    {
        HatDataHandler.currentHatIndex = newHatIndex;
    }
}