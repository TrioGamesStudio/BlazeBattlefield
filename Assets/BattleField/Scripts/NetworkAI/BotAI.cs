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
    [SerializeField] private float dropBoxDetectionRadius = 50f;
    [SerializeField] private float playerDetectionRadius = 30f;
    [SerializeField] private float collectDistance = 2f;
    [SerializeField] private LayerMask dropBoxLayer;
    [SerializeField] private LayerMask playerLayer;

    private NavMeshAgent agent;
    private BotState currentState;
    private Transform currentTarget;
    Animator anim;
    private int currentPointIndex;

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

                //case BotState.MovingToDropBox:
                //    HandleMovingToDropBox();
                //    break;

                //case BotState.CollectingDropBox:
                //    HandleCollectingDropBox();
                //    break;

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
        //// Exit current state logic
        //switch (currentState)
        //{
        //    case BotState.PatrollingPath:
        //        isWaitingAtPoint = false;
        //        StopAllCoroutines();
        //        break;
        //}

        currentState = newState;

        //// Enter new state logic
        //switch (newState)
        //{
        //    case BotState.PatrollingPath:
        //        StartCoroutine(StateBehaviorRoutine());
        //        break;
        //}

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
        //CheckForDropBox();
    }

    private void MoveToNextRoutePoint()
    {
        //currentPointIndex = (currentPointIndex + 1) % routePoints.Length;
        agent.SetDestination(routePoints[currentPointIndex].position);
        currentPointIndex = Random.Range(0, routePoints.Length);
        anim.SetFloat("walkSpeed", agent.speed);
    }

    //private void HandleSearchingDropBox()
    //{
    //    Collider[] dropBoxes = Physics.OverlapSphere(transform.position, dropBoxDetectionRadius, dropBoxLayer);

    //    if (dropBoxes.Length > 0)
    //    {
    //        // Find closest drop box
    //        Transform closestDropBox = null;
    //        float closestDistance = float.MaxValue;

    //        foreach (Collider dropBox in dropBoxes)
    //        {
    //            float distance = Vector3.Distance(transform.position, dropBox.transform.position);
    //            if (distance < closestDistance)
    //            {
    //                closestDistance = distance;
    //                closestDropBox = dropBox.transform;
    //            }
    //        }

    //        if (closestDropBox != null)
    //        {
    //            currentTarget = closestDropBox;
    //            SetState(BotState.MovingToDropBox);
    //        }
    //    }
    //    else
    //    {
    //        // No drop boxes found, start patrolling
    //        SetState(BotState.PatrollingPath);
    //    }
    //}

    //private void HandleMovingToDropBox()
    //{
    //    if (currentTarget == null)
    //    {
    //        SetState(BotState.SearchingDropBox);
    //        return;
    //    }

    //    agent.SetDestination(currentTarget.position);
    //    float distance = Vector3.Distance(transform.position, currentTarget.position);
    //    anim.SetFloat("walkSpeed", agent.speed);

    //    if (distance <= collectDistance)
    //    {
    //        SetState(BotState.CollectingDropBox);
    //    }
    //}

    
}
