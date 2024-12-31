using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.AI;
using System;

public class BotAINetwork : NetworkBehaviour, IStateAuthorityChanged
{
    public enum BotState
    {
        Idle,
        FollowingRoute,
        MovingToDropBox,
        CollectingDropBox,
        ReturningToRoute,
        FacingAndFiring,
    }

    [Networked]
    private bool IsMoving { get; set; }

    [Networked]
    private bool HasGunNetworked { get; set; }

    [Networked]
    private Vector3 TargetPosition { get; set; }

    [Networked]
    private int CurrentPointIndex { get; set; }

    [Networked]
    private bool IsFiring { get; set; }

    [Networked]
    private BotState CurrentNetworkedState { get; set; } 

    [Header("Route Settings")]
    [SerializeField] private Transform[] routePoints;
    [SerializeField] private float pointReachDistance = 2f;

    [Header("Detection Settings")]
    [SerializeField] private float dropBoxDetectionRadius;
    [SerializeField] private float playerDetectionRadius;
    [SerializeField] private float collectDistance;
    [SerializeField] private float itemCollectRadius;
    [SerializeField] private LayerMask dropBoxLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask itemLayer;
    [SerializeField] private float fireRate = 0.5f;

    private float fireCooldownTimer = 0f;
    private bool isInitialized = false;
    private NetworkRunner runner;

    public bool HasGun;
    private NavMeshAgent agent;
    private BotState currentState;
    private Transform currentTarget;
    private Transform currentDropBox;
    private Animator anim;
    private int currentPointIndex;


    #region SETUP BOT

    public override void Spawned()
    {
        base.Spawned();
        runner = Object.Runner;

        // Initialize networked properties
        HasGun = false;
        HasGunNetworked = false;
        CurrentNetworkedState = BotState.Idle;
        TargetPosition = transform.position; // Initialize with current position

        if (Object.HasStateAuthority)
        {
            InitializeBot();
        }
        SetState(BotState.Idle);
    }

    private void InitializeBot()
    {
        if (isInitialized) return;

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        SetState(BotState.FollowingRoute);
        StartCoroutine(StateBehaviorRoutine());

        isInitialized = true;
        Debug.Log($"///Bot initialized by authority: {runner.LocalPlayer}");
    }

    public void SetRoutePoints(Transform[] points)
    {
        if (points == null || points.Length == 0)
        {
            Debug.LogError("///Invalid route points provided!");
            return;
        }

        routePoints = points;
        CurrentPointIndex = UnityEngine.Random.Range(0, routePoints.Length);

        if (routePoints[CurrentPointIndex] != null)
        {
            TargetPosition = routePoints[CurrentPointIndex].position;
        }

        SetState(BotState.FollowingRoute);
    }

    private void RestoreBotState()
    {
        currentState = CurrentNetworkedState;
        HasGun = HasGunNetworked;

        if (agent != null && TargetPosition != default)
        {
            agent.SetDestination(TargetPosition);
        }
    }

    // Update method to sync position
    public override void FixedUpdateNetwork()
    {
        IsMoving = agent != null && agent.velocity.magnitude > 0.1f;
        HasGunNetworked = HasGun;

        if (agent != null && agent.hasPath)
        {
            TargetPosition = agent.destination;
        }
    }

    private void Update()
    {
        // Reduce the cooldown timer over time
        if (fireCooldownTimer > 0f)
        {
            fireCooldownTimer -= Time.deltaTime;
        }
    }

    #endregion

    #region REQUEST AND CHANGE STATE AUTHORITY

    public void StateAuthorityChanged()
    {
        if (Object.HasStateAuthority)
        {
            Debug.Log($"///Gained authority over bot by ${Object.StateAuthority}");
            InitializeBot();
            RestoreBotState();
        }
        else
        {
            Debug.Log($"///Lost authority over bot: {Object.Id}");
            StopAllCoroutines();
            isInitialized = false;
        }
    }

    // Logic to request authority
    public void RequestAuthority()
    {
        //if (!Object.HasStateAuthority)
        {
            Object.RequestStateAuthority();
            Debug.Log($"///Requesting state authority for bot {gameObject.name}.");
        }
    }

    #endregion

    #region BOT FINITE STATE MACHINE

    private void SetState(BotState newState)
    {
        currentState = newState;
        CurrentNetworkedState = newState;
        Debug.Log($"///Bot state changed to: {newState}");
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
                    ExecuteCollectingDropBox();
                    break;
                case BotState.FacingAndFiring:
                    ExecuteFacingAndFiringState();
                    break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    #endregion

    #region BOT IDLE STATE

    void Idle()
    {
        // Bot is idle
    }

    #endregion

    #region BOT MOVING STATE

    private void ExecuteFollowingRouteState()
    {
        //if (Object != null && !Object.HasStateAuthority) return;

        if (routePoints == null || routePoints.Length == 0)
        {
            Debug.Log("/// no routes");
            return;
        }


        if (!agent.pathPending && agent.remainingDistance <= pointReachDistance)
        {
            MoveToNextRoutePoint();
        }

        if (!HasGun)
        {
            CheckForDropBox();
        }
        else
        {
            CheckForPlayer();
        }
    }

    private void MoveToNextRoutePoint()
    {
        CurrentPointIndex = UnityEngine.Random.Range(0, routePoints.Length);
        agent.SetDestination(routePoints[CurrentPointIndex].position);
        TargetPosition = routePoints[CurrentPointIndex].position;
        anim.SetFloat("walkSpeed", agent.speed);
    }

    private void ExecuteReturningToRouteState()
    {
        if (!agent.pathPending && agent.remainingDistance <= pointReachDistance)
        {
            SetState(BotState.FollowingRoute);
        }
    }

    #endregion

    #region BOT COLLECTING STATE

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
                if (!IsPositionOnNavMesh(dropBox.transform.position, 1f)) continue;
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

        float distanceToDropBox = Vector3.Distance(transform.position, currentDropBox.position);
        if (distanceToDropBox <= collectDistance)
        {
            SetState(BotState.CollectingDropBox);
            //StartCoroutine(CollectingRoutine());
        }
    }

    private void ExecuteCollectingDropBox()
    {
        Debug.Log("///Bot collect box");
        // Get items in range and store their game objects
        List<GameObject> itemsToCollect = new();
        Collider[] items = Physics.OverlapSphere(transform.position, itemCollectRadius, itemLayer);

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
            if (item != null && item.TryGetComponent(out GunItem itemCollect) && !HasGun)
            {
                itemCollect.CollectAI(GetComponent<ActiveWeaponAI>()); // Collect and equip gun
                Debug.Log("/// Collected" + item.name);
                HasGun = true;
                SetState(BotState.ReturningToRoute);
                break; // Found gun, exit
            }
        }

        // After check all items, return to the route
        SetState(BotState.ReturningToRoute);
    }

    private bool IsPositionOnNavMesh(Vector3 position, float radius)
    {
        NavMeshHit hit;
        // Check if the position is on the NavMesh within the given radius
        bool isOnNavMesh = NavMesh.SamplePosition(position, out hit, radius, NavMesh.AllAreas);

        return isOnNavMesh;
    }

    #endregion

    #region BOT FIRING STATE

    private void CheckForPlayer()
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
        // If the target player is lost, return to the route
        if (currentTarget == null)
        {         
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

        //Target die, return to route
        if (currentTarget.TryGetComponent<CheckBodyParts>(out var targetHP))
        {
            if (targetHP.hPHandler.Networked_IsDead)
            {
                SetState(BotState.ReturningToRoute);
                return;
            }
        }

        //Bot die, not cotinue to fire
        if (GetComponent<HPHandler>().Networked_HP <= 0)
        {
            SetState(BotState.Idle);
        }

        //Chase player
        agent.SetDestination(currentTarget.position);

        FaceTarget(currentTarget);

        FireGunAtPlayer(currentTarget);

    }

    private void FaceTarget(Transform target)
    {
        transform.LookAt(target);
    }

    private void FireGunAtPlayer(Transform target)
    {
        //Look at player
        //agent.SetDestination(Vector3.forward);

        // Check if the bot is ready to fire
        if (fireCooldownTimer <= 0f)
        {
            Debug.Log("Firing gun at player: " + target.name);

            // Your gun firing logic
            ActiveWeaponAI weapon = GetComponent<ActiveWeaponAI>();
            if (weapon != null)
            {
                weapon.Fire(target);
            }

            // Reset the cooldown timer
            fireCooldownTimer = fireRate;
        }
    }

    #endregion
}
