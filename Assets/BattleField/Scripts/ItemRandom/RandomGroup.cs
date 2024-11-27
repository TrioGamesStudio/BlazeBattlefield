using Fusion;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGroup : NetworkBehaviour
{
    [SerializeField] private DropBox dropBoxPrefab;
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
    [SerializeField] private bool isRefreshChild = false;
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
            CreateValidSpawnPoints();
            GenerateItem();
        }
    }


    private void GenerateItem()
    {
        
        int count = Random.Range(3, 7);
        foreach (var validSpawnPos in validSpawnPoints)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-sizeOfLocation.x / 2, sizeOfLocation.x / 2),
                Random.Range(-sizeOfLocation.y / 2, sizeOfLocation.y / 2),
                Random.Range(-sizeOfLocation.z / 2, sizeOfLocation.z / 2)
            );
            Vector3 spawnPosition = validSpawnPos.position + randomOffset;

            var networkObject = Runner.Spawn(dropBoxPrefab, spawnPosition);
            networkObject.transform.SetParent(validSpawnPos);
        }
        Debug.Log($"This check point is create {validSpawnPoints.Count}");
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
        Debug.Log($"Create {validSpawnPoints.Count} valid point");
    }


#if UNITY_EDITOR
    [Button]
    private void ToggleDrawChildSpawn()
    {
        drawAllChildLocation = !drawAllChildLocation;
    }
    [Button]
    private void ToggleDrawBoundOfGroup()
    {
        drawBoundOfGroup = !drawBoundOfGroup;
    }
    [Button]
    private void ToggleDrawValidSpawnPoint()
    {
        drawValidSpawnPoint = !drawValidSpawnPoint;
    }

    private void OnDrawGizmos()
    {
        if (Application.isEditor == false) return;

        if (drawAllChildLocation)
        {
            
            foreach (var child in spawnPoints)
            {
                if (child == null)
                {
                    GetSpawnPointInChild();
                    break;
                }
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(child.transform.position, child.transform.position + Vector3.up);
                Gizmos.color = childColor;
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
                if (child == null)
                {
                    GetSpawnPointInChild();
                    break;
                }
                bounds.Encapsulate(child.transform.position);
            }
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        if (drawValidSpawnPoint)
        {
            Gizmos.color = validSpawnPointColor;
            foreach (var child in validSpawnPoints)
            {
                if (child == null)
                {
                    GetSpawnPointInChild();
                    break;
                }
                Gizmos.DrawWireCube(child.transform.position, sizeOfLocation);
            }
        }
    }
#endif


}
