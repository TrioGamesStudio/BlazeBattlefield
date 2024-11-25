using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private float minValue;
    [SerializeField] private float maxValue;
    [SerializeField] private Slider healthSlider;

    public void SetMaxHealthAmount(float amount)
    {
        maxValue = amount;
    }

    public void OnHealthChange(float changeAmount)
    {     
        healthSlider.value = (maxValue - changeAmount) / maxValue;
    }
}
