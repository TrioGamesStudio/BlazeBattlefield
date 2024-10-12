using UnityEngine;

public static class Utils
{
    //random spawnPoint Player
    public static Vector3 GetRandomSpawnPoint() {
        return new Vector3(Random.Range(-10, 10), 5, Random.Range(0, 5));
    }

    // random spawnPoint Weapons
    public static Vector3 GetRandomWeaponSpawnPoint() {
        return new Vector3(Random.Range(-10, 18), 1, Random.Range(0, 8));
    }
    
    // set layer for model (or for player's body) make sure cam not render this layer
    public static void SetRenderLayerInChildren(Transform transform, int layerNum) {
        foreach (var trans in transform.GetComponentsInChildren<Transform>(true))
        {
            //todo neu transform co tag = IgnoreLayerChange => cay sung ko bi xet thanh Layer "LocalPlayerModel"
            //todo giu nguyen cay sung la layer Default=> localCam render thay (localCam ko render "LocalPlayerModel" | culling mask setting)
            if(trans.CompareTag("IgnoreLayerChange"))
                continue;

            trans.gameObject.layer = layerNum;
        }
    }
}
