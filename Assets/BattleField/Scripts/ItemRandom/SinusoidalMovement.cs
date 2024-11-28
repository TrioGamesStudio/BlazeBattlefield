using System.Collections;
using UnityEngine;

public class SinusoidalMovement : MonoBehaviour
{
    public float timePeriod = 2;
    public float height = 30f;

    private float timeSinceStart;

    [SerializeField] private Vector3 pivot;
    private SinusoidalMovementManager manager;
    private void Start()
    {
        pivot = transform.localPosition;
        height /= 2;
        timeSinceStart = (3 * timePeriod) / 4;

        manager = SinusoidalMovementManager.instance;
        if (manager != null)
        {
            manager.Register(gameObject);
        }
    }
    private void OnDestroy()
    {
        if (manager != null)
        {
            manager.UnRegister(gameObject);
        }
    }

    public void UpdateSinPosition(float deltaTime)
    {
        Vector3 nextPos = transform.localPosition;
        nextPos.y = pivot.y + height + height * Mathf.Sin(((Mathf.PI * 2) / timePeriod) * timeSinceStart);

        timeSinceStart += deltaTime;
        transform.localPosition = nextPos;
    }
    public void SetPivot(Vector3 newPosition)
    {
        //pivot = newPosition;
    }
}
