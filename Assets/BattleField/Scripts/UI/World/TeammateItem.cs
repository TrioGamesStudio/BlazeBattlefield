using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TeammateItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI teammateName;
    [SerializeField] private TextMeshProUGUI hpText;

    public void SetTeammateInfo(string name, int hp)
    {
        teammateName.text = name;
        hpText.text = hp.ToString();
    }
}
