using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropAmountUI : MonoBehaviour
{
    [SerializeField] private Slider amountSlider;
    [SerializeField] private Button IncreaseButton;
    [SerializeField] private Button DecreaseButton;
    [SerializeField] private Button CancelButton;
    [SerializeField] private Button OkButton;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI currentCountDrop;
    [SerializeField] private TextMeshProUGUI maxCount;
    public Action<int> OnDropCallback;

    private void Awake()
    {
        IncreaseButton.onClick.AddListener(Increase);
        DecreaseButton.onClick.AddListener(Decrease);
        CancelButton.onClick.AddListener(Cancel);
        OkButton.onClick.AddListener(Drop);

        amountSlider.onValueChanged.AddListener(HandleSliderValueChange);
    }
    private void OnDestroy()
    {
        IncreaseButton.onClick.RemoveListener(Increase);
        DecreaseButton.onClick.RemoveListener(Decrease);
        CancelButton.onClick.RemoveListener(Cancel);
        OkButton.onClick.RemoveListener(Drop);

        amountSlider.onValueChanged.RemoveListener(HandleSliderValueChange);

    }
    private void HandleSliderValueChange(float value)
    {
        currentCountDrop.text = value.ToString();
    }
    public void SetupView(ItemData itemData)
    {
        int currentCount = itemData.GetCount();
        string _itemName = itemData.GetItemName();

        amountSlider.minValue = 1;
        amountSlider.maxValue = currentCount;
        amountSlider.value = amountSlider.minValue;
        itemName.text = _itemName;
        maxCount.text = currentCount.ToString();
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
        OnDropCallback?.Invoke((int)amountSlider.value);
    }
}
