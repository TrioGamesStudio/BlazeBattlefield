using UnityEngine;

public static class Utils
{
    //random spawnPoint Player
    public static Vector3 GetRandomSpawnPoint() {
        return new Vector3(Random.Range(-2, 0), 5, Random.Range(-2, -2));
    }

    // random spawnPoint Weapons
    public static Vector3 GetRandomWeaponSpawnPoint() {
        return new Vector3(Random.Range(-10, 18), 1, Random.Range(0, 8));
    }
    
    // set layer for model (or for player's body) make sure cam not render this layer
    public static void SetRenderLayerInChildren(Transform transform, int layerNum) {
        foreach (var trans in transform.GetComponentsInChildren<Transform>(true))
        {
            if(trans.CompareTag("IgnoreLayerChange"))
                continue;

            trans.gameObject.layer = layerNum;
        }
    }
}
