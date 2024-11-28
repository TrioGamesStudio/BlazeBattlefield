using Fusion;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGroupManager : NetworkBehaviour
{
    public static event Action StartSpawnEvent;

    [SerializeField] private GameObject container;
    [SerializeField] private DropBox dropBoxPrefab;
    [Header("Settings")]
    [SerializeField, Min(1)] private int totalBoxCountInAllRegion;
    [SerializeField] private RandomGroup[] randomGroups;

    public static void RaiseStartSpawnEvent()
    {
        StartSpawnEvent?.Invoke();
    }
    public override void Spawned()
    {
        base.Spawned();
        if (Runner.IsSharedModeMasterClient)
        {
        }
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

    [EditorButton]
    private void StartSpawn()
    {
        if (NetworkPlayer.Local.Runner.IsSharedModeMasterClient == false)
        {
            return;
        }

        Debug.Log("Start spawn", gameObject);
        int countPerRegion = totalBoxCountInAllRegion / randomGroups.Length;
        foreach (var group in randomGroups)
        {
            if (group.gameObject.activeSelf == false) continue;
            group.SetDropBoxPrefab(dropBoxPrefab);
            group.SpawnDropBoxesInGroup(countPerRegion);
        }

        Debug.Log($"HAVE TOTAL: {countPerRegion * randomGroups.Length} drop box in area", gameObject);
    }
}
