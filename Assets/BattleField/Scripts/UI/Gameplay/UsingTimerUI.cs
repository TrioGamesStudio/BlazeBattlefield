using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UsingTimer : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private float timer;
    [SerializeField] private float doneTimer;
    [SerializeField] private bool canTimer;
    [Header("UI")]
    private Action onTimerComplete;
    private void Awake()
    {
        timer = 0;
        canTimer = false;
        doneTimer = 0;
    }

    public void StartTimer(float time, Action onTimerComplete)
    {
        doneTimer = time;
        this.onTimerComplete = onTimerComplete;
    }

    public void Cancel()
    {
        canTimer = false;
        onTimerComplete = null;
    }

    private void Update()
    {
        if (timer > 0 && canTimer)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                canTimer = false;
                timer = 0;
                onTimerComplete?.Invoke();
                onTimerComplete = null;
            }
        }
    }
}
public class UsingTimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Button cancelButton;

    private void UpdateTimerText(float timer)
    {
    }
}
