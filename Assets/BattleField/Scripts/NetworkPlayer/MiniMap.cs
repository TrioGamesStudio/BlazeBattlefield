using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Camera miniMapCam_Player; // cam chay theo de hien thi tren mini map
    //GameObject miniMapTeamMate;

    private void Awake()
    {
        miniMapCam_Player.enabled = true;
        //miniMapTeamMate.SetActive(true);
    }
    private void LateUpdate()
    {
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y,0f);
    }
}
