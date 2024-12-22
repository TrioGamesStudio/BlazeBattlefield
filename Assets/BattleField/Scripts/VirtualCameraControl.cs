using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCameraControl : MonoBehaviour
{
    private Dictionary<string, VirtualCamera> virtualsCamera = new();
    private static VirtualCameraControl instance;
    public static VirtualCameraControl Instance
    {
        get => TryCreateInstance();
    }
    private int hightOrder = 5;
    private int lowOrder = 0;

    public void SetCameraOrderToFirst(string cameraName)
    {
        if (virtualsCamera == null)
        {
            Debug.Log("Have no camera register in this scene", gameObject);
            return;
        }

        if (virtualsCamera.TryGetValue(cameraName, out var camera))
        {
            Debug.Log($"Finded camera register in this scene: {camera.cameraName}", gameObject);

            camera.cinemachine.Priority = hightOrder;

            foreach (var item in virtualsCamera)
            {
                if (item.Key == camera.cameraName) continue;
                item.Value.cinemachine.Priority = lowOrder;
            }
        }
        else
        {
            Debug.LogError($"Your camera name {cameraName} does not exit !!!", gameObject);
        }

    }

    public void Add(VirtualCamera VirtualCamera)
    {

        string key = VirtualCamera.cameraName;
        if (string.IsNullOrEmpty(key) == false) return;
        if (!virtualsCamera.ContainsKey(VirtualCamera.cameraName))
        {
            virtualsCamera.Add(key, VirtualCamera);
        }
    }

    public void Remove(VirtualCamera VirtualCamera)
    {
        string key = VirtualCamera.cameraName;
        if (string.IsNullOrEmpty(key) == false) return;
        if (virtualsCamera.ContainsKey(VirtualCamera.cameraName))
        {
            virtualsCamera.Remove(key);
        }
    }

    private static VirtualCameraControl TryCreateInstance()
    {
        if (instance == null)
        {
            var go = new GameObject();
            go.name = "Virtual Camera Control";
            instance = go.AddComponent<VirtualCameraControl>();
        }

        return instance;
    }
}
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

