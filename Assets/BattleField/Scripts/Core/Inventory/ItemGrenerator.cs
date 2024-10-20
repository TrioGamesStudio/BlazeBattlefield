using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemGrenerator : NetworkBehaviour
{
    public ItemDataSO itemDataSo;
    public int count = 3;

    public ItemInGame itemPrefab;
    // Start is called before the first frame update
    public override void Spawned()
    {
        base.Spawned();
        Debug.Log("Spawned");
        for (int i = 0; i < count; i++)
        {
            ItemInGame item = Runner.Spawn(itemPrefab,RandomPosition(),Quaternion.identity);
            item.transform.SetParent(transform);
            item.Setup(itemDataSo,Random.Range(1, 5));
        }
    }

    private Vector3 RandomPosition()
    {
        return new Vector3(Random.Range(-1f, 1), 0,Random.Range(-1f, 1));
    }

}
