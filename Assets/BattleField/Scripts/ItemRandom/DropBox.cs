using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropBox : MonoBehaviour
{
    [SerializeField] private Transform crate_Lid;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward);
    }

    [Button]
    private void Open()
    {
        //crate_Lid.transform.eulerAngles = new Vector3(-90, 0, 0);
        StartCoroutine(OpenCoroutine());
    }
    
    [Button]
    private void Close()
    {
        crate_Lid.transform.eulerAngles = new Vector3(0, 0, 0);
    }

    private IEnumerator OpenCoroutine()
    {
        YieldInstruction yieldInstruction = new WaitForEndOfFrame();
        while (true)
        {
            crate_Lid.transform.eulerAngles = Vector3.Lerp(crate_Lid.transform.eulerAngles, new Vector3(-90, 0, 0), Time.deltaTime);
            yield return yieldInstruction;
        }
    }
}
