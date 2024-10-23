using Fusion;
using UnityEngine;

public class ItemGrenerator : NetworkBehaviour
{
    public int count = 3;

    // Start is called before the first frame update
    public override void Spawned()
    {
        base.Spawned();
        Debug.Log("Spawned");
        for (int i = 0; i < count; i++)
        {
            ItemGeneratorManager.instance.CreateRandomItemFromSource();
        }
    }

    

}
