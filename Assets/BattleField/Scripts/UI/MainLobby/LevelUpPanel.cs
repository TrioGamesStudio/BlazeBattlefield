using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI levelUpText; // "Level Up!" Text 
    [SerializeField] private Image oldRankImage;         // Old Rank Icon
    [SerializeField] private Image newRankImage;
    [SerializeField] Sprite[] rankIcons;

    [SerializeField] private float textZoomDuration = 0.5f;
    [SerializeField] private float rankTransitionDuration = 1.0f;

    private void Start()
    {
        //ShowLevelUpPanel(1, 2);
    }

    public void ShowLevelUpPanel(int oldRank, int newRank)
    {
        Debug.Log("+++" + oldRank + newRank);
        levelUpText.transform.localScale = Vector3.zero; // Hidden
        panel.SetActive(true);


        // Set initial state
        oldRankImage.sprite = rankIcons[oldRank];
        newRankImage.sprite = rankIcons[newRank];
        newRankImage.color = new Color(1, 1, 1, 0); // Transparent

        // Animate the level-up text
        levelUpText.transform.DOScale(1.5f, textZoomDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                levelUpText.transform.DOScale(1.0f, 0.2f); // Settle at normal size
            });

        // Animate the rank transition
        Sequence rankTransition = DOTween.Sequence();
        rankTransition.Append(oldRankImage.transform.DOScale(0, rankTransitionDuration / 2)
            .SetEase(Ease.InBack)); // Shrink old rank
        rankTransition.AppendCallback(() =>
        {
            oldRankImage.color = new Color(1, 1, 1, 0); // Make old rank transparent
            newRankImage.color = new Color(1, 1, 1, 1); // Make new rank visible
        });
        rankTransition.Append(newRankImage.transform.DOScale(1.2f, rankTransitionDuration / 2)
            .SetEase(Ease.OutBack)); // Grow new rank
    }
}
