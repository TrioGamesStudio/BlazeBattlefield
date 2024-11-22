using Fusion;
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


    [EditorButton]
    private void GenerateItem()
    {
        List<Transform> validSpawnItem = new();
        int spawnCount = spawnPoints.Count / 3;
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
        int count = Random.Range(3, 7);
        foreach (var validSpawnPos in validSpawnItem)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 randomOffset = new Vector3(
                Random.Range(-sizeOfLocation.x / 2, sizeOfLocation.x / 2),
                Random.Range(-sizeOfLocation.y / 2, sizeOfLocation.y / 2),
                Random.Range(-sizeOfLocation.z / 2, sizeOfLocation.z / 2)
            );
                Vector3 spawnPosition = validSpawnPos.position + randomOffset;
                var item = Runner.Spawn(ItemDatabase.instance.GetRandomItemPrefab(), spawnPosition);
                item.transform.SetParent(transform);
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
