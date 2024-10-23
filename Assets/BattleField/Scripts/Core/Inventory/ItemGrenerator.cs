using Fusion;
using NaughtyAttributes;
using UnityEngine;

public class ItemGrenerator : NetworkBehaviour
{
    public int count = 3;

    // Start is called before the first frame update
    public override void Spawned()
    {
        base.Spawned();
        Debug.Log("Spawned");
        if (Runner.IsSharedModeMasterClient == false) return;
        for (int i = 0; i < count; i++)
        {
            ItemGeneratorManager.instance.CreateRandomItemFromSource();
        }
    }
    [Button]
    public void Create()
    {
        ItemGeneratorManager.instance.CreateRandomItemFromSource();
    }

}
