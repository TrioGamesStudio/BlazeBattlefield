using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class CharacterHatHandler : NetworkBehaviour
{
    [Networked, SerializeField] private byte hatIndex { get; set; }
    public Transform hatAttach;

    public override void Spawned()
    {
        base.Spawned();

    }

    public void RandomHat()
    {

    }

    public void SpawnHat()
    {

    }
}
