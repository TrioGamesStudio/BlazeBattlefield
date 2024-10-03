using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PlayerData", menuName = "PlayerData")]
public class PlayerData : ScriptableObject
{
    private static string[] names = new string[]
 {
    "John",
    "Alice",
    "Michael",
    "Emma",
    "David",
    "Sophia",
    "James",
    "Olivia",
    "Robert",
    "Isabella"
 };
    public string PlayerName;

    public void CreateRandomPlayerName()
    {
        PlayerName = names[Random.Range(0, names.Length)];
    }
}
