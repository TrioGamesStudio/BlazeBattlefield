using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamMemberPanel : MonoBehaviour
{
    public Transform player;  // Reference to the player transform
    public Vector3 offset;    // Offset for positioning the health bar above the player

    private RectTransform panelRect; // RectTransform of the health bar UI
    private Camera mainCamera;

    void Start()
    {
        // Get reference to the main camera and the RectTransform of the health bar
        mainCamera = Camera.main;
        panelRect = GetComponent<RectTransform>();
    }

    public void Setup()
    {
        if (player != null)
        {
            // Convert player's world position to screen position
            Vector3 screenPos = mainCamera.WorldToScreenPoint(player.position + offset);

            // Update the health bar UI position based on the screen position
            panelRect.position = screenPos;
        }
    }

    public void LeaveRoom()
    {
        FindObjectOfType<Matchmaking>().LeaveRoom();

        // neu roi dou thi ko show button
        FindObjectOfType<SkinSelection>().ToggleSelectSkinButton(true);
        
    }

    void Update()
    {
        if (player != null)
        {
            // Convert player's world position to screen position
            Vector3 screenPos = mainCamera.WorldToScreenPoint(player.position + offset);

            // Update the health bar UI position based on the screen position
            panelRect.position = screenPos;
        }
    }
}
