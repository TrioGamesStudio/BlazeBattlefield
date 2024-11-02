using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RoomPanelUI : MonoBehaviour
{
    public TextMeshProUGUI roomCapacity;
    public TextMeshProUGUI roomNameText;
    string roomName;

    public void SetRoomButtonUI(string infomation, string roomName)
    {
        roomCapacity.text = infomation;
        roomNameText.text = roomName;
        this.roomName = roomName;
    }

    public void JoinRoom()
    {
        UIController.Instance.HidePanel();
        FindObjectOfType<Matchmaking>().JoinRoomByName(roomName);
    }

    public void Deactive(bool isFull)
    {
        GetComponentInChildren<Button>().interactable = !isFull;
    }
}
