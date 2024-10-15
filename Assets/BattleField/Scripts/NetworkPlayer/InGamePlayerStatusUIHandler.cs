using TMPro;
using UnityEngine;

public class InGamePlayerStatusUIHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerHP;

    public void OnGamePlayerHpRecieved(byte hp) {
        playerHP.text = $"HP: {hp.ToString()}" ;
    }
}
