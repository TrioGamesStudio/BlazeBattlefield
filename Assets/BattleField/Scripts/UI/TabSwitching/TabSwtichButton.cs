using System;
using UnityEngine;
using UnityEngine.UI;

public class TabSwtichButton : MonoBehaviour
{
    public Button btn;
    public byte tabIndex;
    public Action<byte> OnShowTabUI;
    
    [SerializeField] private Image normalIcon;
    [SerializeField] private Image focusIcon;
    [SerializeField] private Image focusPanel;

    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        btn.onClick.RemoveListener(OnClick);
    }

    public void SetIcon(Sprite newIcon)
    {
        normalIcon.sprite = newIcon;
        focusIcon.sprite = newIcon;
    }

    private void OnClick()
    {
        OnShowTabUI?.Invoke(tabIndex);
    }

    public void Interactable()
    {
        btn.interactable = true;
        focusPanel.gameObject.SetActive(false);
    }

    public void UnInteractable()
    {
        btn.interactable = false;
        focusPanel.gameObject.SetActive(true);
    }
}