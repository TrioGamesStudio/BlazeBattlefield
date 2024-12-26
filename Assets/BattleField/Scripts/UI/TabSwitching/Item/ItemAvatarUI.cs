using System;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemAvatarUI : MonoBehaviour
{
    public TextMeshProUGUI information;
    public TextMeshProUGUI displayName;
    public Image icon;
    [SerializeField] private float hightlightTime = .1f;
    [SerializeField] private float unHighlightTime = .1f;
    [SerializeField] private Image hightlightFrame;
    [SerializeField] private Button btn;
    public int ItemIndex;
    public event Action<int> OnClickUI;
    [Button]
    public void Highlight()
    {
        hightlightFrame.DOFade(1, hightlightTime);
    }

    [Button]
    public void UnHightlight()
    {
        hightlightFrame.DOFade(0, unHighlightTime);
    }

    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnRaiseEvent);
    }
    private void OnRaiseEvent()
    {
        OnClickUI?.Invoke(ItemIndex);
    }
}