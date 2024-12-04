using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
public class TouchRotationView : MonoBehaviour
{
    private RectTransform rectTransform;
    [SerializeField] private bool isLooking;
    [SerializeField] private int touchID;
    [SerializeField] private Vector2 delta;
    private void Awake()
    {
        EnhancedTouchSupport.Enable();
        rectTransform = GetComponent<RectTransform>();

    }
    private void Update()
    {
        var activeTouches = Touch.activeTouches;
        //for (var i = 0; i < activeTouches.Count; ++i)
            //Debug.Log("Active touch: " + activeTouches[i]);

        if(activeTouches.Count > 0)
        {
            foreach(var touch in activeTouches)
            {
                switch (touch.phase)
                {
                    case UnityEngine.InputSystem.TouchPhase.None:
                        break;
                    case UnityEngine.InputSystem.TouchPhase.Began:
                        Check(touch);
                        break;
                    case UnityEngine.InputSystem.TouchPhase.Moved:
                        Move(touch);
                        break;
                    case UnityEngine.InputSystem.TouchPhase.Ended:
                        End(touch);
                        break;
                    case UnityEngine.InputSystem.TouchPhase.Canceled:
                        break;
                    case UnityEngine.InputSystem.TouchPhase.Stationary:
                        Static(touch);
                        break;
                    default:
                        break;
                }
            }
        }
    }
  

    private void Check(Touch touch)
    {
        if (isLooking == true) return;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            touch.screenPosition,
            null,
            out localPoint
        );
        if (rectTransform.rect.Contains(localPoint))
        {
            Debug.Log("Touched inside image!");
            isLooking = true;
            touchID = touch.touchId;

        }
    }
    private void Move(Touch touch)
    {
        if(isLooking && touch.touchId == touchID)
        {
            delta = touch.delta;
            InputPlayerMovement.LookAction?.Invoke(delta);

        }
    }
    private void End(Touch touch)
    {
        if (isLooking && touch.touchId == touchID)
        {
            delta = Vector2.zero;
            isLooking = false;
            touchID = -1;
            InputPlayerMovement.LookAction?.Invoke(delta);
        }
    }
    private void Static(Touch touch)
    {
        if (isLooking && touch.touchId == touchID)
        {
            delta = Vector2.zero;
            InputPlayerMovement.LookAction?.Invoke(delta);

        }
    }
}