using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class TriggerOutline : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Outline outline))
        {
            StartCoroutine(LerpColor(outline, true,255));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Outline outline))
        {
            StartCoroutine(LerpColor(outline, false, 0));

        }
    }
    [SerializeField] private float speed = .5f;
    private IEnumerator LerpColor(Outline outline, bool enable,float value)
    {
        var targetColor = outline.OutlineColor;
        targetColor.a = value;
        while (outline.OutlineColor.a == targetColor.a)
        {
            outline.OutlineColor = Color.Lerp(outline.OutlineColor, targetColor, Time.deltaTime * speed);
            yield return new WaitForEndOfFrame();
        }
        outline.enabled = enable;
    }
}
