using Cinemachine;
using UnityEngine;

public class VirtualCamera : MonoBehaviour
{
    public string cameraName;
    public CinemachineVirtualCamera cinemachine;
    private void Awake()
    {
        cinemachine = GetComponent<CinemachineVirtualCamera>();
        VirtualCameraControl.Instance.Add(this);
    }

    private void OnDestroy()
    {
        VirtualCameraControl.Instance.Remove(this);
    }
}
