using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGroupManager : MonoBehaviour
{
    public static RandomGroupManager instance;
    [SerializeField] private GameObject container;
    [SerializeField] private DropBox dropBoxPrefab;
    [Header("Settings")]
    [SerializeField,Min(1)] private int totalBoxCountInAllRegion;
    [SerializeField] private RandomGroup[] randomGroups;
    
    private void Awake()
    {
        randomGroups = container.GetComponentsInChildren<RandomGroup>();
    }

    [Button]
    public void StartSpawn()
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
