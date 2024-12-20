using System;
using UnityEngine;
using UnityEngine.UI;

public class SkinAvatarUI : MonoBehaviour
{
    public Action<int> OnClickUI;
    public Button button;
    public Image avatarImg;
    private void Awake()
    {
        button.onClick.AddListener(RaiseEvent);
    }
    private void OnDestroy()
    {
        button.onClick.RemoveListener(RaiseEvent);
    }

    private void RaiseEvent()
    {
        OnClickUI?.Invoke(transform.GetSiblingIndex());
    }
}
