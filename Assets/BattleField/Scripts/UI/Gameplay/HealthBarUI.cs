using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class HealthBarUI : MonoBehaviour
{
    public static Action<float> OnHealthChangeAction;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider healthRegenSlider;
    [SerializeField] private Slider healthLostSlider;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private float minValue;
    [SerializeField] private float maxValue;
    [Header("Settings")] 
    [SerializeField] private float healthLostDelay = .5f;

    private void Awake()
    {
        SetMinMaxValue(healthSlider);
        SetMinMaxValue(healthRegenSlider);
        SetMinMaxValue(healthLostSlider);

        healthSlider.value = maxValue;
        healthRegenSlider.value = minValue;
        healthLostSlider.value = minValue;
        
        healthText.text = maxValue.ToString();

        OnHealthChangeAction += OnHealthChange;
    }

    private void OnDestroy()
    {
        OnHealthChangeAction -= OnHealthChange;

    }


    // Clamp value in slider
    private void SetMinMaxValue(Slider slider)
    {
        slider.minValue = minValue;
        slider.maxValue = maxValue;
    }

    [Button]
    private void Test()
    {
        OnHealthChange(Random.Range(0, 101));
    }

    public void OnHealthChange(float newHealthValue)
    {
        CancelInvoke();
        var currentHealthValue = healthSlider.value;

        if (newHealthValue > currentHealthValue)
        {
            // health
        }
        else
        {
            // show lost health slider
        }
        healthSlider.value = newHealthValue;
        healthLostSlider.value = currentHealthValue;
        healthText.text = newHealthValue.ToString();
        Debug.Log($"Health change debug: current health {newHealthValue} ; old value {currentHealthValue}", gameObject);
        Invoke(nameof(DelayTest), healthLostDelay);
    }

    private void DelayTest()
    {
        healthLostSlider.value = healthSlider.value;
    }


}