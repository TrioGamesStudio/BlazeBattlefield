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
        Firing,
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

    [SerializeField] Transform playerModel;

    [Header("Route Settings")]
    [SerializeField] private Transform[] routePoints;
    [SerializeField] private float pointReachDistance;

    [Header("Detection Settings")]
    [SerializeField] private float dropBoxDetectionRadius;    
    [SerializeField] private float collectDistance;
    [SerializeField] private float itemCollectRadius;
    [SerializeField] private float playerDetectionRadius;
    [SerializeField] private float fireDistance;
    [SerializeField] private LayerMask dropBoxLayer;   
    [SerializeField] private LayerMask itemLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float fireRate;

    [Header("Evasion Settings")]
    [SerializeField] private float evadeDistance = 2f; // Distance for evasion moves
    [SerializeField] private float evadeInterval = 1f; // Time between evasion actions
    [SerializeField] private float jumpHeight = 2f; // Height for jumps
    [SerializeField] private float evadeSpeed = 3f; // Speed for evasion
    private float lastEvadeTime = 0f; // Timer to track evasion intervals

    private float fireCooldownTimer = 0f;
    private bool isInitialized = false;
    private NetworkRunner runner;

    public bool HasGun;
    private NavMeshAgent agent;
    private HPHandler hpHandler;
    private BotState currentState;
    private Transform currentTarget;
    private Transform currentDropBox;
    private Animator anim;
    //private int currentPointIndex;
    //private Transform targetPosition;

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
        hpHandler = GetComponent<HPHandler>();
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
        if (!Object.HasStateAuthority) return;
        if (hpHandler.Networked_HP <= 0)
        {
            StopAllCoroutines();
            return;
        }
        // Reduce the cooldown timer over time
        if (fireCooldownTimer > 0f)
        {
            fireCooldownTimer -= Time.deltaTime;
        }
    }

    #endregion

    #region REQUEST AND CHANGE STATE AUTHORITY

    // Logic to request authority
    public void RequestAuthority()
    {
        if (Object == null)
        {
            Debug.Log($"///Object is null or destroyed on {gameObject.name}");
            return;
        }

        if (!Object.HasStateAuthority)
        {
            try
            {
                Object.RequestStateAuthority();
                Debug.Log($"///Requesting state authority for bot {gameObject.name}.");
            }
            catch (Exception ex)
            {
                Debug.Log($"///Failed to request state authority: {ex.Message}");
            }
        }
        else
        {
            Debug.Log("///Object already has state authority.");
        }
    }

    public void StateAuthorityChanged()
    {
        if (Object.HasStateAuthority)
        {
            Debug.Log($"///Gained authority over bot by ${Object.StateAuthority}");
            InitializeBot();
            RestoreBotState();
            AlivePlayerControl.UpdateAliveCount(-1);
        }
        else
        {
            Debug.Log($"///Lost authority over bot: {Object.Id}");
            StopAllCoroutines();
            isInitialized = false;
        }
    }

    #endregion

    #region BOT FINITE STATE MACHINE

    private void SetState(BotState newState)
    {
        currentState = newState;
        CurrentNetworkedState = newState;
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
                case BotState.CollectingDropBox:
                    ExecuteCollectingDropBox();
                    break;
                case BotState.ReturningToRoute:
                    ExecuteReturningToRouteState();
                    break;
                case BotState.Firing:
                    ExecuteFiringState();
                    break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    #endregion

    #region BOT IDLE STATE

    void Idle()
    {
        Debug.Log("/// Bot idle");
    }

    #endregion

    #region BOT MOVING STATE

    private void ExecuteFollowingRouteState()
    {
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
            CheckForItem();
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
        //if (!agent.pathPending && agent.remainingDistance <= pointReachDistance)
        {
            MoveToNextRoutePoint();
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
                if (!IsPositionOnNavMesh(dropBox.transform.position, 1f)) continue; //Drop box not inside navmesh area
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
        }
    }

    private void ExecuteCollectingDropBox()
    {
        Debug.Log("///Bot collect box");
        Collider[] items = Physics.OverlapSphere(playerModel.position, itemCollectRadius, itemLayer);
        Debug.Log("///Found item qty: " + items.Length);

        foreach (var item in items)
        {
            Debug.Log("/// Item" + item.name);
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

    private void CheckForItem()
    {
        Debug.Log("///Bot check for item");
        Collider[] items = Physics.OverlapSphere(playerModel.position, itemCollectRadius, itemLayer);
        Debug.Log("///Found item qty: " + items.Length);

        foreach (var item in items)
        {
            Debug.Log("/// Item" + item.name);
            if (item != null && item.TryGetComponent(out GunItem itemCollect) && !HasGun)
            {
                itemCollect.CollectAI(GetComponent<ActiveWeaponAI>()); // Collect and equip gun
                Debug.Log("/// Collected" + item.name);
                HasGun = true;
                break; // Found gun, exit
            }
        }
    }

    private bool IsPositionOnNavMesh(Vector3 position, float radius)
    {
        // Check if the position is on the NavMesh within the given radius
        bool isOnNavMesh = NavMesh.SamplePosition(position, out NavMeshHit hit, radius, NavMesh.AllAreas);

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
            currentTarget = playersInRange[0].transform; // Save the player as the target

            // Transition to the FacingAndFiring state
            SetState(BotState.Firing);
          
        }
    }

    private void ExecuteFiringState()
    {
        // If the target player is lost, return to the route
        if (currentTarget == null)
        {
            agent.isStopped = false;
            SetState(BotState.FollowingRoute);
            return;
        }

        //Target die, return to route
        if (currentTarget.TryGetComponent<CheckBodyParts>(out var targetHP))
        {
            if (targetHP.hPHandler.Networked_IsDead)
            {
                agent.isStopped = false;
                currentTarget = null;
                SetState(BotState.ReturningToRoute);
                return;
            }
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

        if (distanceToPlayer > fireDistance)
        {
            agent.isStopped = false;
            //Chase player
            agent.SetDestination(currentTarget.position);
        }
        else
        {
            //agent.SetDestination(playerModel.forward);
            agent.isStopped = true;

            FaceAtTarget(currentTarget);

            // Perform evasion behavior
            HandleEvasion();

            FireAtTarget(currentTarget);
        }         
    }

    private void FaceAtTarget(Transform target)
    {
        transform.LookAt(target);
    }

    private void FireAtTarget(Transform target)
    {
        //Look at player
        //agent.SetDestination(Vector3.forward);
        //Transform offsetFiringPos = new Vector3(0, 1, 0);
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

    private void HandleEvasion()
    {
        if (Time.time - lastEvadeTime < evadeInterval) return; // Limit evasion frequency

        lastEvadeTime = Time.time;

        // Choose a random evasion action
        int action = UnityEngine.Random.Range(0, 5);

        switch (action)
        {
            case 0: // Step backward
                Vector3 backward = transform.position - transform.forward * evadeDistance;
                StartCoroutine(MoveToPosition(backward));
                break;
            case 1: // Step forward
                Vector3 forward = transform.position + transform.forward * evadeDistance;
                StartCoroutine(MoveToPosition(forward));
                break;
            case 2: // Strafe left
                Vector3 left = transform.position - transform.right * evadeDistance;
                StartCoroutine(MoveToPosition(left));
                break;
            case 3: // Strafe right
                Vector3 right = transform.position + transform.right * evadeDistance;
                StartCoroutine(MoveToPosition(right));
                break;
            case 4: // Jump
                StartCoroutine(Jump());
                break;
        }
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        float duration = evadeDistance / evadeSpeed;

        Vector3 startPosition = transform.position;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }

    private IEnumerator Jump()
    {
        float jumpTime = 0.5f; // Duration of the jump
        float elapsedTime = 0f;

        Vector3 startPos = transform.position;

        while (elapsedTime < jumpTime)
        {
            float jumpProgress = elapsedTime / jumpTime;
            float jumpOffset = Mathf.Sin(jumpProgress * Mathf.PI) * jumpHeight; // Sin curve for smooth jump
            transform.position = new Vector3(startPos.x, startPos.y + jumpOffset, startPos.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Return to the ground
        transform.position = startPos;
    }
    #endregion

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        Debug.Log("/// Despawn Bot AI called for runner: " + runner.name);
        StopAllCoroutines();
    }
}
