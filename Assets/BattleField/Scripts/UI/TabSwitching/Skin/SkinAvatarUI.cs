using System;
using UnityEngine;
using UnityEngine.UI;

public class SkinAvatarUI : MonoBehaviour
{
    public Action<int> OnClickUI;
    public Button button;
    public Image avatarImg;
    public GameObject innerFrame;
    public GameObject selectFrame;
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
        OnClickUI?.Invoke(transform.GetSiblingIndex());
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
}
