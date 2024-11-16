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
    [Button]
    private void Test()
    {
        StartTimer(2, () => { Debug.Log("Timer Complete"); });
    }

    public void StartTimer(float time, Action onTimerComplete)
    {
        this.onTimerComplete = onTimerComplete;
        timer = time;
        maxTimer = time;
        onProcess = true;
        TimerActionUI.instance.Show();
    }

    public void Cancel()
    {
        onTimerComplete = null;
        onProcess = false;
        TimerActionUI.instance.Hide();
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
                onTimerComplete = null;
                TimerActionUI.instance.Hide();
            }

            TimerActionUI.instance.UpdateTimerText(Math.Round(timer, 1));
            TimerActionUI.instance.Fade(timer / maxTimer);
        }
    }
}
