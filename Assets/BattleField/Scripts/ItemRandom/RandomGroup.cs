using Fusion;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGroup : NetworkBehaviour
{
    [SerializeField] private DropBox dropBoxPrefab;
    [SerializeField] private GameObject dropBoxContainer;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private List<Transform> validSpawnPoints;
    //[SerializeField] private int maxItemBoxInGroup = 10;
    //[SerializeField] private int validSpawnPointCount;
    [Header("Gizmos Settings")]
    [SerializeField] private Color groupColor = Color.white;
    [SerializeField] private Color childColor = Color.red;
    [SerializeField] private Color validSpawnPointColor = Color.blue;
    [SerializeField] private Vector3 sizeOfLocation = Vector3.one;
    [SerializeField] private bool drawBoundOfGroup = false;
    [SerializeField] private bool drawAllChildLocation = false;
    [SerializeField] private bool drawValidSpawnPoint = false;
    [SerializeField] private bool isRefreshChild = false;

    private int spawnCount = 1;

    public void SetDropBoxPrefab(DropBox newDropBoxPrefab)
    {
        dropBoxPrefab = newDropBoxPrefab;
    }

    public void SpawnDropBoxesInGroup(int _spawnCount)
    {
        if (dropBoxPrefab == null)
        {
            Debug.LogError("Please add drop box prefab before spawning", gameObject);
            return;
        }
        spawnCount = _spawnCount;
        if (validSpawnPoints.Count == 0)
        {
            SelectValidSpawnPoints();
        }
        SpawnDropBoxes();
    }

    private void SpawnDropBoxes()
    {
        foreach (var validSpawnPos in validSpawnPoints)
        {
            var networkObject = Runner.Spawn(dropBoxPrefab, validSpawnPos.position);
            networkObject.transform.SetParent(dropBoxContainer.transform);
        }
        Debug.Log($"This check point is create {validSpawnPoints.Count}");
    }

    [EditorButton]
    private void SelectValidSpawnPoints()
    {
        validSpawnPoints.Clear();
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

    #region Gizmos

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
    [EditorButton]
    private void GetSpawnPointInChild()
    {
        spawnPoints.Clear();
        foreach (Transform child in transform)
        {
            if (child.name.Equals("DROP_BOX_CONTAINER")) continue;
            spawnPoints.Add(child);
        }
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

    #endregion
}
