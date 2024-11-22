using Fusion;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGroup : NetworkBehaviour
{
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private int maxItems = 10;
    [Header("Gizmos Settings")]
    [SerializeField] private Color groupColor = Color.white;
    [SerializeField] private Color childColor = Color.red;
    [SerializeField] private bool drawBoundOfGroup = false;
    [SerializeField] private Vector3 sizeOfLocation = Vector3.one;
    [SerializeField] private bool drawChildLocation = false;
    [Button]
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
        GenerateItem();
    }


    [Button]
    private void GenerateItem()
    {
        List<Transform> validSpawnItem = new();
        int spawnCount = spawnPoints.Count / 4;
        HashSet<int> usedIndices = new();

        while (validSpawnItem.Count < spawnCount)
        {
            int randomIndex = Random.Range(0, spawnPoints.Count);
            if (!usedIndices.Contains(randomIndex))
            {
                usedIndices.Add(randomIndex);
                validSpawnItem.Add(spawnPoints[randomIndex]);
            }
        }
        int count = Random.Range(1, 3);
        foreach (var validSpawnPos in validSpawnItem)
        {
            for (int i = 0; i < count; i++)
            {
                Runner.Spawn(ItemDatabase.instance.GetRandomItemPrefab(), validSpawnPos.position);
            }
        }

    }

    private void OnDrawGizmos()
    {
        if (drawChildLocation)
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
    }
}
