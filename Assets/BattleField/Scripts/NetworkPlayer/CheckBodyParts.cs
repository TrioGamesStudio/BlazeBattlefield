using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBodyParts : MonoBehaviour
{
    public HPHandler hPHandler;

    private void Awake() {
        hPHandler = GetComponentInParent<HPHandler>();
    }


}
