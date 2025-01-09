#if UNITY_EDITOR
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSkinData : MonoBehaviour
{
    public SkinDataHandler skinDataHandler;
    public SkinSelectionUI skinSelectionUI;

    private void Awake()
    {
        StartCoroutine(LoadWaiting());
    }

    private IEnumerator LoadWaiting()
    {
        yield return new WaitForSeconds(.5f);
        skinSelectionUI.SetDeaultSkin(skinDataHandler.CurrentSkinIndex);
    }
}
