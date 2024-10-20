using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyCanvas : MonoBehaviour
{
    private void Awake()
    {
        // Check if there is already a canvas with this tag to avoid duplicates
        if (FindObjectsOfType<DontDestroyCanvas>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        // Make this GameObject persistent across scenes
        DontDestroyOnLoad(gameObject);
    }
}
