using UnityEngine;
using UnityEngine.EventSystems;

public class DriveHover : MonoBehaviour,IPointerDownHandler, IPointerUpHandler
{
    private DriveUI driveUI;
    public DriveDirection DriveDirection;
    private void Awake()
    {
        driveUI = GetComponentInParent<DriveUI>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        driveUI.SetTrigger(DriveDirection, true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        driveUI.SetTrigger(DriveDirection, false);
    }
}