using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGroupManager : MonoBehaviour
{
    public static event Action StartSpawnEvent;

    [SerializeField] private GameObject container;
    [SerializeField] private DropBox dropBoxPrefab;
    [Header("Settings")]
    [SerializeField,Min(1)] private int totalBoxCountInAllRegion;
    [SerializeField] private RandomGroup[] randomGroups;

    public static void RaiseStartSpawnEvent()
    {
        StartSpawnEvent?.Invoke();
    }

    private void Awake()
    {
        randomGroups = container.GetComponentsInChildren<RandomGroup>();

        StartSpawnEvent += StartSpawn;
    }
    private void OnDestroy()
    {
        StartSpawnEvent -= StartSpawn;
    }

    [Button]
    private void StartSpawn()
    {
        Debug.Log("Start spawn",gameObject);
        int countPerRegion = totalBoxCountInAllRegion / randomGroups.Length;
        foreach(var group in randomGroups)
        {
            group.SetDropBoxPrefab(dropBoxPrefab);
            group.SpawnDropBoxesInGroup(countPerRegion);
        }

        Debug.Log($"HAVE TOTAL: {countPerRegion * randomGroups.Length} drop box in area",gameObject);
    }
}
