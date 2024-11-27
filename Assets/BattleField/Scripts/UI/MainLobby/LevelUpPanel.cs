using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    public void ShowLevelUpPanel(int oldRank, int newRank)
    {
        Debug.Log("+++" + oldRank + newRank);
        panel.SetActive(true);
    }
}
