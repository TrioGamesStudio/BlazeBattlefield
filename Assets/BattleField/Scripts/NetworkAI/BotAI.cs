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
        FacingAndFiring,
        Idle
    }

    [Header("Route Settings")]
    [SerializeField] private Transform[] routePoints;
    [SerializeField] private float pointReachDistance = 2f;

    [Header("Detection Settings")]
    [SerializeField] private float dropBoxDetectionRadius;
    [SerializeField] private float playerDetectionRadius;
    [SerializeField] private float collectDistance;
    [SerializeField] private LayerMask dropBoxLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask itemLayer;
    private float moveToDropBoxStartTime;
    private float maxMoveToDropBoxDuration = 5f; // Time limit in seconds


    private NavMeshAgent agent;
    private BotState currentState;
    private Transform currentTarget;
    private Transform currentDropBox;
    Animator anim;
    private int currentPointIndex;
    private bool hasGun = false;
    // Cooldown timer for firing
    private float fireCooldownTimer = 0f;

    // Firing rate in seconds (e.g., 0.5 seconds means 2 shots per second)
    [SerializeField] private float fireRate = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetState(BotState.Idle);
        StartCoroutine(StateBehaviorRoutine());
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // Reduce the cooldown timer over time
        if (fireCooldownTimer > 0f)
        {
            fireCooldownTimer -= Time.deltaTime;
        }
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
                    //agent.isStopped = false;
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

                case BotState.FacingAndFiring:
                    //agent.isStopped = true; // Stop moving while firing
                    //anim.SetFloat("walkSpeed", 0);
                    ExecuteFacingAndFiringState();
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
        //agent.isStopped = false;
        if (routePoints == null || routePoints.Length == 0) return;
        // Check if reached current route point
        if (!agent.pathPending && agent.remainingDistance <= pointReachDistance)
        {
            MoveToNextRoutePoint();
        }

        // Look for drop boxes while following route
        if (!hasGun)
        {
            CheckForDropBox();
        }       
        else
        {
            // Detect players and handle shooting behavior
            CheckForPlayerAndFire();
        }
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
                moveToDropBoxStartTime = Time.time; // Reset timer when starting to move
                SetState(BotState.MovingToDropBox);            
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

    private void CheckForPlayerAndFire()
    {
        // Detect players within the detection radius
        Collider[] playersInRange = Physics.OverlapSphere(transform.position, playerDetectionRadius, playerLayer);

        if (playersInRange.Length > 0)
        {
            // Transition to the FacingAndFiring state
            SetState(BotState.FacingAndFiring);
            currentTarget = playersInRange[0].transform; // Save the player as the target
        }
    }

    private void ExecuteFacingAndFiringState()
    {
        if (currentTarget == null)
        {
            // If the target player is lost, return to the route
            SetState(BotState.FollowingRoute);
            return;
        }

        // Check if the player is still in detection range
        float distanceToPlayer = Vector3.Distance(transform.position, currentTarget.position);
        if (distanceToPlayer > playerDetectionRadius)
        {
            // If the player is out of range, return to the route
            SetState(BotState.FollowingRoute);
            currentTarget = null;
            return;
        }

        //agent.isStopped = true;
        // Face the player
        FaceTarget(currentTarget);

        // Check if the bot is ready to fire
     
        FireGunAtPlayer(currentTarget);

    }

    private void FaceTarget(Transform target)
    {
        //Vector3 direction = (target.position - transform.position).normalized;
        //direction.y = 0; // Keep the bot level on the ground
        //Quaternion lookRotation = Quaternion.LookRotation(direction);
        //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // Smoothly rotate towards the target
        transform.LookAt(target);
    }

    private void FireGunAtPlayer(Transform target)
    {
        // Check if the bot is ready to fire
        if (fireCooldownTimer <= 0f)
        {
            Debug.Log("Firing gun at player: " + target.name);

            // Your gun firing logic
            ActiveWeaponAI weapon = GetComponent<ActiveWeaponAI>();
            if (weapon != null)
            {
                weapon.Fire(target); // Replace with your firing logic
            }

            // Reset the cooldown timer
            fireCooldownTimer = fireRate;
        }
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
