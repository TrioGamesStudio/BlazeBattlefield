using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCameraTest : MonoBehaviour
{
    public string cameraName;

    [Button]
    private void Test()
    {
        VirtualCameraControl.Instance.SetCameraOrderToFirst(cameraName);
    }
}
