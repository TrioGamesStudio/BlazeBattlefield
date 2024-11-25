using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private float minValue;
    [SerializeField] private float maxValue;
    [SerializeField] private Slider healthSlider;
    private float currentHealth;

    public void SetMaxHealthAmount(float amount)
    {
        maxValue = amount;
        currentHealth = amount;
    }

    public void OnHealthChange(float changeAmount)
    {
        currentHealth -= changeAmount;
        healthSlider.value = currentHealth / maxValue;
    }
}
