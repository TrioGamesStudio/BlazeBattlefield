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
}