using System;
using UnityEngine;
using UnityEngine.UI;

public class TabSwtichButton : MonoBehaviour
{
    public Button btn;
    public byte tabIndex;
    public Action<byte> OnShowTabUI;
    
    public Image normalIcon;
    public Image focusIcon;
    public Image focusPanel;
    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        btn.onClick.RemoveListener(OnClick);
    }
    private void OnClick()
    {
        OnShowTabUI?.Invoke(tabIndex);
    }

    public void Focus()
    {

    }

    public void UnFocus()
    {

    }
}