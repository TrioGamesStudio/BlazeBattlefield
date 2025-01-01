using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class CharacterHatHandler : NetworkBehaviour
{
    [Networked, SerializeField] private int hatIndex { get; set; }
    [SerializeField] private HatDataHandler HatDataHandler;
    [SerializeField] private Transform hatTransform;
    public override void Spawned()
    {
        base.Spawned();
    }
    [EditorButton]
    private void CreateHat()
    {
        var prefab = HatDataHandler.GetHatPrefabByIndex(hatIndex);

        if (prefab == null)
        {
            Debug.Log("Hat prefab is null", gameObject);
            return;
        }

        Instantiate(prefab, Vector2.zero, Quaternion.identity, hatTransform);
    }
}
