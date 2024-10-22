using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyCanvas : MonoBehaviour
{
    private void Awake()
    {
        // Make this GameObject persistent across scenes
        DontDestroyOnLoad(gameObject);
    }
}
