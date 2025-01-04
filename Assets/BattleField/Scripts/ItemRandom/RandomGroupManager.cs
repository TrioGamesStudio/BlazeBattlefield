using Fusion;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGroupManager : NetworkBehaviour
{
    public static RandomGroupManager instance;
    public static event Action StartSpawnEvent;
    public static Action OnRequestStateAuthority;
    [SerializeField] private GameObject container;
    [SerializeField] private DropBox dropBoxPrefab;
    [Header("Settings")]
    [SerializeField, Min(1)] private int totalBoxCountInAllRegion;
    [SerializeField] private RandomGroup[] randomGroups;
    public List<NetworkObject> dropBoxesSpawned = new();
    public static void RaiseStartSpawnEvent()
    {
        StartSpawnEvent?.Invoke();
    }

    private void Awake()
    {
        instance = this;
        randomGroups = container.GetComponentsInChildren<RandomGroup>();

        StartSpawnEvent += StartSpawn;
        OnRequestStateAuthority += RequestStateAuthority;
    }
    private void OnDestroy()
    {
        StartSpawnEvent -= StartSpawn;
        OnRequestStateAuthority -= RequestStateAuthority;
    }

    [EditorButton]
    private void StartSpawn()
    {
        if (NetworkPlayer.Local.Runner.IsSharedModeMasterClient || NetworkPlayer.Local.Runner.IsSinglePlayer)
        {
            Debug.Log("Start spawn", gameObject);
            int countPerRegion = totalBoxCountInAllRegion / randomGroups.Length;
            foreach (var group in randomGroups)
            {
                if (group.gameObject.activeSelf == false) continue;
                group.SetDropBoxPrefab(dropBoxPrefab);
                group.SpawnDropBoxesInGroup(countPerRegion, Runner);
            }
            Debug.Log($"HAVE TOTAL: {countPerRegion * randomGroups.Length} drop box in area", gameObject);

        }

    }

    public void RequestStateAuthority()
    {
        if (Object == null)
        {
            Debug.Log($"///Object is null or destroyed on {gameObject.name}");
            return;
        }

        if (!Object.HasStateAuthority)
        {
            try
            {
                Object.RequestStateAuthority();
                
                Debug.Log($"///Requesting state authority for bot {gameObject.name}.");
            }
            catch (Exception ex)
            {
                Debug.Log($"///Failed to request state authority: {ex.Message}");
            }
            foreach (var item in dropBoxesSpawned)
            {
                item.RequestStateAuthority();
            }
        }
        else
        {
            Debug.Log("///Object already has state authority.");
        }
    }

    public void AddItem(NetworkObject networkObject)
    {
        if (!dropBoxesSpawned.Contains(networkObject))
        {
            dropBoxesSpawned.Add(networkObject);
        }
    }

    public void RemoveItem(NetworkObject networkObject)
    {
        if (dropBoxesSpawned.Contains(networkObject))
        {
            dropBoxesSpawned.Remove(networkObject);
        }
    }
}
