using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BotAI : MonoBehaviour
{
    public enum BotState
    {
        FollowingRoute,
        MovingToDropBox,
        CollectingDropBox,
        ReturningToRoute,
        Idle
    }

    [Header("Route Settings")]
    [SerializeField] private Transform[] routePoints;
    [SerializeField] private float pointReachDistance = 2f;

    [Header("Detection Settings")]
    [SerializeField] private float dropBoxDetectionRadius = 5f;
    [SerializeField] private float playerDetectionRadius = 30f;
    [SerializeField] private float collectDistance = 0.2f;
    [SerializeField] private LayerMask dropBoxLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask itemLayer;
    private float moveToDropBoxStartTime;
    private float maxMoveToDropBoxDuration = 3f; // Time limit in seconds


    private NavMeshAgent agent;
    private BotState currentState;
    private Transform currentTarget;
    private Transform currentDropBox;
    Animator anim;
    private int currentPointIndex;
    private bool hasGun = false;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetState(BotState.Idle);
        StartCoroutine(StateBehaviorRoutine());
        anim = GetComponentInChildren<Animator>();
    }

    // Public method to assign route points
    public void SetRoutePoints(Transform[] points)
    {
        if (points == null || points.Length == 0)
        {
            Debug.LogError("Invalid route points provided!");
            return;
        }

        routePoints = points;
        currentPointIndex = Random.Range(0, routePoints.Length);
        SetState(BotState.FollowingRoute);

    }

    private IEnumerator StateBehaviorRoutine()
    {
        while (true)
        {
            switch (currentState)
            {
                case BotState.Idle:
                    Idle();
                    break;

                case BotState.FollowingRoute:
                    ExecuteFollowingRouteState();
                    break;

                case BotState.MovingToDropBox:
                    ExecuteMovingToDropBoxState();
                    break;

                case BotState.ReturningToRoute:
                    ExecuteReturningToRouteState();
                    break;

                case BotState.CollectingDropBox:
                    HandleCollectingDropBox();
                    break;

                    //case BotState.PatrollingPath:
                    //    HandlePatrollingPath();
                    //    break;

                    //case BotState.ChasingPlayer:
                    //    HandleChasingPlayer();
                    //    break;

            }

            // Always check for players except when collecting
            //if (currentState != BotState.CollectingDropBox)
            //{
            //    CheckForPlayers();
            //}

            yield return new WaitForSeconds(0.1f); // Update interval
        }
    }


    private void SetState(BotState newState)
    {
        currentState = newState;

        Debug.Log($"Bot state changed to: {newState}");
    }

    void Idle()
    {
        //agent.enabled = false;
    }

    private void ExecuteFollowingRouteState()
    {
        //agent.enabled = true;
        if (routePoints == null || routePoints.Length == 0) return;
        // Check if reached current route point
        if (!agent.pathPending && agent.remainingDistance <= pointReachDistance)
        {
            MoveToNextRoutePoint();
        }

        // Look for drop boxes while following route
        if(!hasGun)
            CheckForDropBox();
    }

    private void MoveToNextRoutePoint()
    {
        //currentPointIndex = (currentPointIndex + 1) % routePoints.Length;
        agent.SetDestination(routePoints[currentPointIndex].position);
        currentPointIndex = Random.Range(0, routePoints.Length);
        anim.SetFloat("walkSpeed", agent.speed);
    }

    private void CheckForDropBox()
    {
        if (currentState != BotState.FollowingRoute) return;

        Collider[] dropBoxes = Physics.OverlapSphere(transform.position, dropBoxDetectionRadius, dropBoxLayer);

        if (dropBoxes.Length > 0)
        {
            // Find closest drop box
            float closestDistance = float.MaxValue;
            Transform closestDropBox = null;

            foreach (Collider dropBox in dropBoxes)
            {
                float distance = Vector3.Distance(transform.position, dropBox.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestDropBox = dropBox.transform;

                }
            }

            if (closestDropBox != null)
            {
                currentDropBox = closestDropBox;
                SetState(BotState.MovingToDropBox);
                moveToDropBoxStartTime = Time.time; // Reset timer when starting to move
            }
        }

    }

    private void ExecuteMovingToDropBoxState()
    {
        if (currentDropBox == null)
        {
            SetState(BotState.ReturningToRoute);
            return;
        }

        agent.SetDestination(currentDropBox.position);

        // Check if we've exceeded the time limit
        if (Time.time - moveToDropBoxStartTime > maxMoveToDropBoxDuration)
        {
            Debug.Log("Timeout reaching dropbox - returning to route");
            currentDropBox = null;
            SetState(BotState.ReturningToRoute);
            return;
        }

        float distanceToDropBox = Vector3.Distance(transform.position, currentDropBox.position);
        if (distanceToDropBox <= collectDistance)
        {
            SetState(BotState.CollectingDropBox);
            //StartCoroutine(CollectingRoutine());
        }
    }


    private void HandleCollectingDropBox()
    {
        StartCoroutine(CollectItemsWithDelay());
    }

    private IEnumerator CollectItemsWithDelay()
    {
        Debug.Log("...AI collect box");
        anim.SetFloat("walkSpeed", 0);

        // Get items in range and store their game objects
        List<GameObject> itemsToCollect = new();
        Collider[] items = CheckForItems();

        foreach (var item in items)
        {
            // Ensure the item is valid and not destroyed before adding
            if (item != null && item.gameObject != null)
            {
                itemsToCollect.Add(item.gameObject);
            }
        }

        foreach (var item in itemsToCollect)
        {
            if (item != null && item.TryGetComponent(out GunItem itemCollect) && !hasGun)
            {
                itemCollect.CollectAI(GetComponent<ActiveWeaponAI>()); // Collect the item
                Debug.Log("... collect" + item.name);
                hasGun = true;
                SetState(BotState.ReturningToRoute);
                break;
            }

            // Wait for 1 second before collecting the next item
            yield return new WaitForSeconds(1f);
        }

        // After collecting all items, return to the route
        SetState(BotState.ReturningToRoute);
    }

    private void ExecuteReturningToRouteState()
    {
        if (!agent.pathPending && agent.remainingDistance <= pointReachDistance)
        {
            SetState(BotState.FollowingRoute);
        }
    }

    private Collider[] CheckForItems()
    {
        Collider[] items = Physics.OverlapSphere(transform.position, collectDistance, itemLayer);
        return items;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw route
        if (routePoints != null && routePoints.Length > 0)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < routePoints.Length; i++)
            {
                if (routePoints[i] != null)
                {
                    // Draw point
                    Gizmos.DrawSphere(routePoints[i].position, 0.5f);

                    // Draw line to next point
                    if (i + 1 < routePoints.Length && routePoints[i + 1] != null)
                    {
                        Gizmos.DrawLine(routePoints[i].position, routePoints[i + 1].position);
                    }
                    else if (i == routePoints.Length - 1 && routePoints[0] != null)
                    {
                        Gizmos.DrawLine(routePoints[i].position, routePoints[0].position);
                    }
                }
            }
        }

        // Draw detection and collection ranges
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dropBoxDetectionRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, collectDistance);
    }
}
