using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCameraControl : MonoBehaviour
{
    private Dictionary<string, VirtualCamera> virtualsCamera = new();
    private static VirtualCameraControl instance;
    public static VirtualCameraControl Instance
    {
        get
        {
            return instance;
        }
    }
    private int hightOrder = 5;
    private int lowOrder = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SetCameraOrderToFirst(string cameraName)
    {
        cameraName = cameraName.ToLower();
        if (virtualsCamera == null)
        {
            Debug.Log("Have no camera register in this scene", gameObject);
            return;
        }

        if (virtualsCamera.TryGetValue(cameraName, out var camera))
        {
            Debug.Log($"Finded camera register in this scene: {camera.cameraName}", gameObject);

            foreach (var item in virtualsCamera)
            {
                if (item.Key == camera.cameraName) continue;
                item.Value.cinemachine.Priority = lowOrder;
            }
            camera.cinemachine.Priority = hightOrder;
        }
        else
        {
            Debug.LogError($"Your camera name {cameraName} does not exit !!!", gameObject);
        }
    }

    public void Add(VirtualCamera VirtualCamera)
    {
        string key = VirtualCamera.cameraName.ToLower();

        if (!virtualsCamera.ContainsKey(key))
        {
            Debug.Log($"Add new camera {key} ", VirtualCamera.gameObject);
            virtualsCamera.Add(key, VirtualCamera);
        }
    }

    public void Remove(VirtualCamera VirtualCamera)
    {
        string key = VirtualCamera.cameraName.ToLower();

        if (virtualsCamera.ContainsKey(key))
        {
            virtualsCamera.Remove(key);
        }
    }
}
