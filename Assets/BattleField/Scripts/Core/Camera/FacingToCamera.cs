using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FacingToCamera : MonoBehaviour
{
    private Transform cameraTransform;

    private void Start()
    {
        // kiem tra co dang spawn tai ready scene hay khong
        bool isReadyScene = SceneManager.GetActiveScene().name == "MainLobby";
        if (isReadyScene)
        // Find the main camera in the scene
            cameraTransform = Camera.main.transform;
        else
        {
            LocalCameraHandler localCameraHandler = FindObjectOfType<LocalCameraHandler>();
            cameraTransform = localCameraHandler.transform;
        }
    }

    private void LateUpdate()
    {
        // Make the Canvas face the camera
        transform.LookAt(transform.position + cameraTransform.rotation * Vector3.forward, cameraTransform.rotation * Vector3.up);
    }
}
