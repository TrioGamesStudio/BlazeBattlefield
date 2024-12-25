using Cinemachine;
using UnityEngine;

public class VirtualCamera : MonoBehaviour
{
    public string cameraName;
    public CinemachineVirtualCamera cinemachine;
    private void Awake()
    {
        cinemachine = GetComponent<CinemachineVirtualCamera>();
    }

    private void OnDestroy()
    {
    }
}
