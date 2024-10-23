using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyCanvas : MonoBehaviour
{
    public DontDestroyCanvas Instance;

    private void Awake()
    {
        // Check if there is already a canvas with this tag to avoid duplicates
        if (FindObjectsOfType<Matchmaking>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        //// Make this GameObject persistent across scenes
        //DontDestroyOnLoad(gameObject);
        // Check if instance already exists and destroy if duplicate
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            // Set the instance to this object
            Instance = this;
            // Optionally, make the object persistent across scenes
            DontDestroyOnLoad(gameObject);
        }
    }
}
