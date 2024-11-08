using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RoomPanelUI : MonoBehaviour
{
    public TextMeshProUGUI roomCapacity;
    public TextMeshProUGUI roomNameText;
    public TextMeshProUGUI roomMapText;
    string roomName;
    string map;

    public void SetRoomButtonUI(string infomation, string roomName, string map)
    {
        roomCapacity.text = infomation;
        roomNameText.text = roomName;
        roomMapText.text = map;
        this.roomName = roomName;
        this.map = map;
    }

    public void JoinRoom()
    {
        UIController.Instance.HidePanel();
        FindObjectOfType<Matchmaking>().JoinRoomByName(roomName);
        if (map == "Harbour")
        {
            FindObjectOfType<Matchmaking>().SetPlayScene(2);
            FindObjectOfType<UIController>().SetMapText("Map Harbour");
        }
        else if (map == "Desert")
        {
            FindObjectOfType<Matchmaking>().SetPlayScene(3);
            FindObjectOfType<UIController>().SetMapText("Map Desert");
        }
    }

    public void Deactive(bool isFull)
    {
        GetComponentInChildren<Button>().interactable = !isFull;
    }
}
