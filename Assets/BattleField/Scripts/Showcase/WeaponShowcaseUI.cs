using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponShowcaseUI : MonoBehaviour, IPointerDownHandler,IPointerUpHandler,IPointerMoveHandler
{
    public bool canRotate;
    public Vector2 rotateInput;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        canRotate = true;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        rotateInput = eventData.delta.normalized;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        canRotate = false;
        rotateInput = Vector2.zero;
    }

}
