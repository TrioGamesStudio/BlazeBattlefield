using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class CharacterHatHandler : NetworkBehaviour
{
    [Networked, SerializeField] private int playerHatIndex { get; set; }
    [SerializeField] private HatDataHandler HatDataHandler;
    [SerializeField] private Transform hatTransform;
    [SerializeField] private Transform currentHat;
    public override void Spawned()
    {
        base.Spawned();
        if (HasStateAuthority)
        {
            playerHatIndex = HatDataHandler.currentHatIndex;
            CreateHat();
        }
    }
    [EditorButton]
    private void CreateHat()
    {
        var prefab = HatDataHandler.GetHatPrefabByIndex(playerHatIndex);

        if (prefab == null)
        {
            Debug.Log("Hat prefab is null", gameObject);
            return;
        }
        if(currentHat != null)
        {
            Destroy(currentHat.gameObject);
        }
        currentHat = Instantiate(prefab, Vector2.zero, Quaternion.identity, hatTransform).transform;
    }
}
