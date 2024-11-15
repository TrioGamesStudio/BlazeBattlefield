using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class TimerActionUI : MonoBehaviour
{
    public static TimerActionUI instance;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Button cancelButton;
    [SerializeField] private GameObject view;
    [SerializeField] private Image circleFadeImage;
    public static Action OnCancel;
    private void Awake()
    {
        instance = this;
        cancelButton.onClick.AddListener(Cancel);
        view.gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        cancelButton.onClick.RemoveListener(Cancel);
    }

    private void Cancel()
    {
        OnCancel?.Invoke();
    }
    public void Show()
    {
        view.SetActive(true);
    }

    public void Hide()
    {
        view.SetActive(false);
    }

    public void UpdateTimerText(double timer)
    {
        timerText.text = timer.ToString();
    }

    public void Fade(float value)
    {
        circleFadeImage.fillAmount = value;
    }
}
