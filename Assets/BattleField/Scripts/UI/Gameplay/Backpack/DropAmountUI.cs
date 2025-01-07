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
    private Action<int> DropItemAction;

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
        Debug.Log("Setup amount silder UI");
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
    public void SetupView(InventoryItem inventoryItem)
    {
        int currentCount = inventoryItem.amount;
        string _itemName = inventoryItem.displayName;

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
    }
    public void Drop()
    {
        DropItemAction?.Invoke((int)amountSlider.value);
    }

    public void SetAcceptDrop(Action<int> onAcceptDrop)
    {
        DropItemAction = onAcceptDrop;
    }

    private void OnDisable()
    {
        amountSlider.SetValueWithoutNotify(0);
    }
}
