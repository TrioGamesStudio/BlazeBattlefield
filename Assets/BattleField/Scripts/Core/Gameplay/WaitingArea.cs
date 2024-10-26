using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class WaitingArea : NetworkBehaviour
{
    public void ReleasePlayer()
    {
        gameObject.SetActive(false);
    }
}
