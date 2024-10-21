using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropAmountUI : MonoBehaviour
{
    [SerializeField] private Slider amountSlider;
    [SerializeField] private Button IncreaseButton;
    [SerializeField] private Button DecreaseButton;
    [SerializeField] private Button CancelButton;
    [SerializeField] private Button OkButton;
    private Action<float> OnDropCallback;
    private void Awake()
    {
        IncreaseButton.onClick.AddListener(Increase);
        DecreaseButton.onClick.AddListener(Decrease);
        CancelButton.onClick.AddListener(Cancel);
    }
    private void OnDestroy()
    {
        IncreaseButton.onClick.RemoveListener(Increase);
        DecreaseButton.onClick.RemoveListener(Decrease);
        CancelButton.onClick.RemoveListener(Cancel);
    }
    public void Setup(ItemData itemData, Action<float> _OnDropCallback)
    {
        amountSlider.minValue = 1;
        amountSlider.maxValue = itemData.count;
        amountSlider.value = amountSlider.minValue;
        OnDropCallback = _OnDropCallback;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Increase()
    {
        amountSlider.value++;
    }

    public void Decrease()
    {
        amountSlider.value--;
    }
    public void Cancel()
    {
        Hide();
        OnDropCallback = null;
    }
    public void Drop()
    {
        OnDropCallback?.Invoke(amountSlider.value);
    }
}
