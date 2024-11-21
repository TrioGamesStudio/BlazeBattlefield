using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class WaitingArea : MonoBehaviour
{
    public void ReleasePlayer()
    {
        gameObject.SetActive(false);
    }
}
