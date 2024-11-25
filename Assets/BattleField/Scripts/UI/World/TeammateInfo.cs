using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeammateInfo : MonoBehaviour
{
    public GameObject teammemberPrefab;  // Prefab to represent a session in UI
    public Transform teamInfoPanel;    // Parent transform for session buttons

    public void CreateTeammemberInfo(string membername, int hp, HPHandler playerHPHandler)
    {
        GameObject teammateInfo = Instantiate(teammemberPrefab, teamInfoPanel);
        teammateInfo.GetComponent<TeammateItem>().SetTeammateInfo(membername, hp, playerHPHandler);
    }
}
