using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TeammateItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI teammateName;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private HealthBar healthBarUI;
    float currentHP;

    public void SetTeammateInfo(string name, int hp, HPHandler playerHPHandler)
    {
        currentHP = hp;
        teammateName.text = name;
        hpText.text = hp.ToString();
        playerHPHandler.OnTakeDamageEvent.AddListener(UpdateHPText);
        healthBarUI.SetMaxHealthAmount(hp);
    }

    void UpdateHPText(float damageAmount)
    {
        currentHP -= damageAmount;
        hpText.text = currentHP.ToString();
        healthBarUI.OnHealthChange(damageAmount);
    }
}
