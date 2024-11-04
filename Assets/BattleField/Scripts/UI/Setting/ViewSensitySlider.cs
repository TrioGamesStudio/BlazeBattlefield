using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;

public class ViewSensitySlider : MonoBehaviour
{
    PlayerRoomController localPlayer;
    NetworkCharacterController networkCharacter;
    public Slider rotationSpeedSlider;
    // Start is called before the first frame update
    void Start()
    {
        PlayerRoomController[] playerRoomControllers = FindObjectsOfType<PlayerRoomController>();
        foreach (var player in playerRoomControllers)
        {
            if (player.isLocalPlayer)
                localPlayer = player;
        }
        networkCharacter = localPlayer.GetComponent<NetworkCharacterController>();
        rotationSpeedSlider.onValueChanged.AddListener(UpdateRotationSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateRotationSpeed(float value)
    {
        networkCharacter.rotationSpeed = value;
        networkCharacter.viewRotationSpeed = value;
    }
}
