using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetInputCallback : MonoBehaviour
{
    [SerializeField] private InputReader[] inputReaders;
    private void Awake()
    {
        foreach(var input in inputReaders)
        {
            input.SetCallbacks();
        }
    }
}
