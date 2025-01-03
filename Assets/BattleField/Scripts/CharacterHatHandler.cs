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
        }
        CreateHatRemote();
    }

    [EditorButton]
    public void CreateHatRemote()
    {
        CreateHatByIndex(playerHatIndex);
    }
    [EditorButton]
    public void CreateHatLocal()
    {
        CreateHatByIndex(HatDataHandler.currentHatIndex);
    }

    private void CreateHatByIndex(int hatIndex)
    {
        var prefab = HatDataHandler.GetHatPrefabByIndex(hatIndex);

        if (prefab == null)
        {
            Debug.Log("Hat prefab is null", gameObject);
            return;
        }
        if (currentHat != null)
        {
            Destroy(currentHat.gameObject);
        }
        currentHat = Instantiate(prefab, Vector3.zero, Quaternion.identity, hatTransform).transform;
        currentHat.transform.localPosition = Vector3.zero;
        currentHat.transform.localEulerAngles = Vector3.zero;
        Debug.Log(currentHat.transform.position);
    }
}
