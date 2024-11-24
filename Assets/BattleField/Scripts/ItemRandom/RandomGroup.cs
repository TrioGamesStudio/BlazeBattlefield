using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGroup : NetworkBehaviour
{
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private List<Transform> validSpawnPoints;
    [SerializeField] private int maxItemBoxInGroup = 10;
    [SerializeField] private int validSpawnPointCount;
    [Header("Gizmos Settings")]
    [SerializeField] private Color groupColor = Color.white;
    [SerializeField] private Color childColor = Color.red;
    [SerializeField] private Color validSpawnPointColor = Color.blue;
    [SerializeField] private Vector3 sizeOfLocation = Vector3.one;
    [SerializeField] private bool drawBoundOfGroup = false;
    [SerializeField] private bool drawAllChildLocation = false;
    [SerializeField] private bool drawValidSpawnPoint = false;
    [EditorButton]
    private void GetSpawnPointInChild()
    {
        spawnPoints.Clear();
        foreach (Transform child in transform)
        {
            spawnPoints.Add(child);
        }
    }

    public override void Spawned()
    {
        base.Spawned();
        Debug.Log("Spawned in random group",gameObject);
        if (Object.HasStateAuthority)
        {
            Debug.Log("Generate item");
            GenerateItem();
        }
    }


    private void GenerateItem()
    {
        
        int count = Random.Range(3, 7);
        foreach (var validSpawnPos in validSpawnPoints)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 randomOffset = new Vector3(
                Random.Range(-sizeOfLocation.x / 2, sizeOfLocation.x / 2),
                Random.Range(-sizeOfLocation.y / 2, sizeOfLocation.y / 2),
                Random.Range(-sizeOfLocation.z / 2, sizeOfLocation.z / 2)
            );
                Vector3 spawnPosition = validSpawnPos.position + randomOffset;
            }
        }
    }
    [EditorButton]
    private void CreateValidSpawnPoints()
    {
        validSpawnPoints.Clear();
        int spawnCount = Mathf.Clamp(validSpawnPointCount,0, maxItemBoxInGroup);
        HashSet<int> usedIndices = new();

        while (validSpawnPoints.Count < spawnCount)
        {
            int randomIndex = Random.Range(0, spawnPoints.Count);
            if (!usedIndices.Contains(randomIndex))
            {
                usedIndices.Add(randomIndex);
                validSpawnPoints.Add(spawnPoints[randomIndex]);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (drawAllChildLocation)
        {
            Gizmos.color = childColor;
            foreach (var child in spawnPoints)
            {
                Gizmos.DrawWireCube(child.transform.position, sizeOfLocation);
            }
        }

        if (drawBoundOfGroup)
        {
            Gizmos.color = groupColor;
            Bounds bounds = new Bounds();
            bounds.center = transform.position;
            foreach (var child in spawnPoints)
            {
                bounds.Encapsulate(child.transform.position);
            }
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        if (drawValidSpawnPoint)
        {
            Gizmos.color = validSpawnPointColor;
            foreach (var child in validSpawnPoints)
            {
                Gizmos.DrawWireCube(child.transform.position, sizeOfLocation);
            }
        }
    }
}
