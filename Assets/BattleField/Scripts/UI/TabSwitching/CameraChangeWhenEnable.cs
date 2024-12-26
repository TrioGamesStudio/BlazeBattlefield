using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChangeWhenEnable : MonoBehaviour
{
    public string cameraName;
    public void ChangeCamera()
    {
        VirtualCameraControl.Instance.SetCameraOrderToFirst(cameraName);
    }
}
