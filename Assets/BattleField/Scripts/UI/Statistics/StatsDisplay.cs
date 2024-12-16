using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class StatsDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    // UI elements for displaying stats
    [SerializeField] private TextMeshProUGUI totalKillText;
    [SerializeField] private TextMeshProUGUI damageDealtText;
    [SerializeField] private TextMeshProUGUI damageReceivedText;
    [SerializeField] private TextMeshProUGUI healthHealedText;
    [SerializeField] private TextMeshProUGUI survivalTimeText;

    // Method to update the UI with player stats
    public void DisplayStats(PlayerStats stats)
    {
        AnimateText(titleText);
        totalKillText.text = $"Total kill: {stats.TotalKill}";
        damageDealtText.text = $"Damage Dealt: {stats.DamageDealt}";
        damageReceivedText.text = $"Damage Received: {stats.DamageReceived}";
        healthHealedText.text = $"Health Healed: {stats.HealthHealed}";
        survivalTimeText.text = $"Survival Time: {stats.SurvivalTime:F2} seconds";
    }

    // Optional: Clear UI for reuse
    public void ClearStatsDisplay()
    {
        damageDealtText.text = "Damage Dealt: 0";
        damageReceivedText.text = "Damage Received: 0";
        healthHealedText.text = "Health Healed: 0";
        survivalTimeText.text = "Survival Time: 0.00 seconds";
    }

    private void AnimateText(TextMeshProUGUI textElement)
    {
        // Reset the scale to the original size
        textElement.transform.localScale = Vector3.one;

        // Zoom up and down
        textElement.transform.DOScale(Vector3.one * 1.2f, 0.2f) // Scale up to 120% in 0.2 seconds
            .OnComplete(() =>
            {
                textElement.transform.DOScale(Vector3.one, 0.2f); // Scale back to 100% in 0.2 seconds
        });
    }
}
