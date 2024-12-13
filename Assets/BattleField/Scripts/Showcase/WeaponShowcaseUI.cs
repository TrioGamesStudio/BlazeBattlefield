using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponShowcaseUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
{
    public bool canRotate;
    public Vector2 rotateInput;
    public float backSpeed = 2;
    public float rotateSpeed = 3;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(canRotate == false)
        {
            rotateInput = Vector2.Lerp(rotateInput, Vector2.zero,Time.deltaTime * backSpeed);
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        canRotate = true;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (canRotate)
        {
            rotateInput += eventData.delta.normalized * rotateSpeed;
        }
        Debug.Log(rotateInput);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        canRotate = false;
    }

}
