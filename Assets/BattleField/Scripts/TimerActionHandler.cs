using NaughtyAttributes;
using System;
using UnityEngine;

public class TimerActionHandler : MonoBehaviour
{
    public static TimerActionHandler instance;
    [Header("Variables")]
    [SerializeField] private float timer;
    [SerializeField] private bool onProcess;
    public bool OnProcess { get => onProcess; }

    private Action onTimerComplete;
    private Action onTimerFalse;
    private float maxTimer;
    private void Awake()
    {
        timer = 0;
        TimerActionUI.OnCancel += Cancel;
        instance = this;
    }

    private void OnDestroy()
    {
        TimerActionUI.OnCancel -= Cancel;
    }
 
    public void StartTimer(float time, Action onTimerComplete,Action onTimerFalse)
    {
        this.onTimerComplete = onTimerComplete;
        this.onTimerFalse = onTimerFalse;
        timer = time;
        maxTimer = time;
        onProcess = true;
        TimerActionUI.instance.Show();
    }

    public void Cancel()
    {
        onTimerFalse?.Invoke();
        ResetCallback();
        onProcess = false;
        TimerActionUI.instance.Hide();
    }

    private void ResetCallback()
    {
        onTimerFalse = null;
        onTimerComplete = null;
    }

    private void Update()
    {
        if (timer > 0 && onProcess)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                onProcess = false;
                timer = 0;
                onTimerComplete?.Invoke();
                ResetCallback();
                TimerActionUI.instance.Hide();
            }

            TimerActionUI.instance.UpdateTimerText(Math.Round(timer, 1));
            TimerActionUI.instance.Fade(timer / maxTimer);
        }
    }
}
