using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private float minValue;
    [SerializeField] private float maxValue;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private GameObject deathIcon;
    private float currentHealth;

    void Start()
    {
        deathIcon.SetActive(false);
        
    }
    public void SetMaxHealthAmount(float amount)
    {
        maxValue = amount;
        currentHealth = amount;
    }

    public void OnHealthChange(float changeAmount)
    {
        currentHealth -= changeAmount;
        healthSlider.value = currentHealth / maxValue;
        if (currentHealth <= 0)
            deathIcon.SetActive(true);
    }
}
