using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class AliveKillUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI killText;
    [SerializeField] private TextMeshProUGUI aliveText;
    
    public static Action<int> UpdateKillCount;
    public static Action<int> UpdateAliveCount;

    private void Awake()
    {
        UpdateKillCount += UpdateKillText;
        UpdateAliveCount += UpdateAliveText;
        UpdateKillText(0);
        UpdateAliveText(0);
    }
    private void OnDestroy()
    {
        UpdateKillCount -= UpdateKillText;
        UpdateAliveCount -= UpdateAliveText;
    }

    private void UpdateKillText(int killCount)
    {
        killText.text = $"Kill: {killCount}";
        Debug.Log("Kill:" + killCount);
    }

    private void UpdateAliveText(int aliveCount)
    {
        aliveText.text = $"Alive: {aliveCount}";
        Debug.Log("Alive:" + aliveCount);
    }
}
