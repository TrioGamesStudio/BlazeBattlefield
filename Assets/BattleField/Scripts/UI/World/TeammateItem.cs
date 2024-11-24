using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TeammateItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI teammateName;
    [SerializeField] private TextMeshProUGUI hpText;
    int currentHP;

    public void SetTeammateInfo(string name, int hp, HPHandler playerHPHandler)
    {
        currentHP = hp;
        teammateName.text = name;
        hpText.text = hp.ToString();
        playerHPHandler.OnTakeDamageEvent.AddListener(UpdateHPText);
        Debug.Log("+++Assign event HP");
    }

    void UpdateHPText(int damageAmount)
    {
        Debug.Log("+++Reduce HP");
        currentHP -= damageAmount;
        hpText.text = currentHP.ToString();
    }
}
