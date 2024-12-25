using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChangeWhenEnable : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public byte enablePriority = 0;
    public byte disablePriority = 0;
    private void OnEnable()
    {
        virtualCamera.Priority = enablePriority;
    }

    private void OnDisable()
    {
        virtualCamera.Priority = disablePriority;
    }
}
