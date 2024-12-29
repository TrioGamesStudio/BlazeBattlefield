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
        FollowingRoute,
        MovingToDropBox,
        CollectingDropBox,
        ReturningToRoute,
        FacingAndFiring,
        Idle
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

    private float moveToDropBoxStartTime;
    private float maxMoveToDropBoxDuration = 5f;
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
    }

    // Update method to sync position
    public override void FixedUpdateNetwork()
    {
        //if (!Object.HasStateAuthority) return;
        Debug.Log("/// Bot running");
        // Update networked state
        IsMoving = agent != null && agent.velocity.magnitude > 0.1f;
        HasGunNetworked = HasGun;

        if (agent != null && agent.hasPath)
        {
            TargetPosition = agent.destination;
        }
    }

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

    public void SetRoutePoints(Transform[] points)
    {
        //if (!Object.HasStateAuthority) return;

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

    private void SetState(BotState newState)
    {
        //if (!Object.HasStateAuthority) return;

        currentState = newState;
        CurrentNetworkedState = newState;
        Debug.Log($"///Bot state changed to: {newState}");
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

    private IEnumerator StateBehaviorRoutine()
    {
        while (true)
        {
            //if (Object != null && !Object.HasStateAuthority)
            //{
            //    Debug.Log($"/// Not have state authority");
            //    Debug.Log($"/// Current state autthority of bot is " + Object.StateAuthority);
            //    yield return new WaitForSeconds(0.1f);
            //    continue;
            //}

            switch (currentState)
            {
                case BotState.Idle:
                    Idle();
                    break;
                case BotState.FollowingRoute:
                    ExecuteFollowingRouteState();
                    break;
                //case BotState.MovingToDropBox:
                //    ExecuteMovingToDropBoxState();
                //    break;
                //case BotState.ReturningToRoute:
                //    ExecuteReturningToRouteState();
                //    break;
                //case BotState.CollectingDropBox:
                //    HandleCollectingDropBox();
                //    break;
                //case BotState.FacingAndFiring:
                //    ExecuteFacingAndFiringState();
                //    break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    void Idle()
    {
        // Bot is idle
    }

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

        //if (!HasGun)
        //{
        //    CheckForDropBox();
        //    List<GameObject> itemsToCollect = new();
        //    Collider[] items = CheckForItems();

        //    foreach (var item in items)
        //    {
        //        if (item != null && item.gameObject != null)
        //        {
        //            itemsToCollect.Add(item.gameObject);
        //        }
        //    }

        //    foreach (var item in itemsToCollect)
        //    {
        //        if (item != null && item.TryGetComponent(out GunItem itemCollect) && !HasGun)
        //        {
        //            itemCollect.CollectAI(GetComponent<ActiveWeaponAI>());
        //            HasGun = true;
        //            HasGunNetworked = true;
        //            SetState(BotState.ReturningToRoute);
        //            break;
        //        }
        //    }
        //}
        //else
        //{
        //    CheckForPlayerAndFire();
        //}
    }

    private void MoveToNextRoutePoint()
    {
        CurrentPointIndex = UnityEngine.Random.Range(0, routePoints.Length);
        agent.SetDestination(routePoints[CurrentPointIndex].position);
        TargetPosition = routePoints[CurrentPointIndex].position;
        anim.SetFloat("walkSpeed", agent.speed);
    }

    // Logic to release authority (optional)
    public void ReleaseAuthority()
    {
        if (Object.HasStateAuthority)
        {
            Object.ReleaseStateAuthority();
            Debug.Log($"///Bot {gameObject.name} released state authority.");
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

    //// Triggered when state authority changes
    //public void OnStateAuthorityChanged(PlayerRef oldAuthority, PlayerRef newAuthority)
    //{
    //    Debug.Log($"Bot {gameObject.name} state authority changed from {oldAuthority} to {newAuthority}.");
    //}
}
