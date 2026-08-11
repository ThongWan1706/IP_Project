using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NormalPedestrianTraffic : MonoBehaviour
{
    public enum CrossingDirection
    {
        Random,
        Straight,
        Right
    }

    [Header("References")]
    public NavMeshAgent agent;
    public Animator animator;

    [Tooltip("Assign the GREEN pedestrian/crossing light.")]
    public Light crossingGreenLight;

    [Header("Route")]
    public Transform stopPoint;
    public Transform straightDestination;
    public Transform rightDestination;

    public CrossingDirection crossingDirection =
        CrossingDirection.Random;

    [Header("Respawn")]
    public Transform respawnPoint;
    public float respawnDelay = 2f;

    [Header("Movement")]
    public float walkSpeed = 2f;
    public float stoppingDistance = 0.25f;
    public float stopPointCheckDistance = 0.6f;
    public float walkingAnimationThreshold = 0.05f;

    private bool reachedStopPoint = false;
    private bool crossingStarted = false;
    private bool finished = false;
    private bool respawning = false;

    private void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (agent == null)
        {
            Debug.LogError(
                gameObject.name + " has no NavMeshAgent."
            );

            enabled = false;
            return;
        }

        agent.speed = walkSpeed;
        agent.stoppingDistance = stoppingDistance;

        if (!agent.isOnNavMesh)
        {
            Debug.LogError(
                gameObject.name + " is not on the NavMesh."
            );

            enabled = false;
            return;
        }

        if (stopPoint == null)
        {
            Debug.LogError(
                gameObject.name + " has no Stop Point."
            );

            enabled = false;
            return;
        }

        StartRoute();
    }

    private void Update()
    {
        if (agent == null ||
            !agent.isOnNavMesh ||
            respawning)
        {
            return;
        }

        if (!reachedStopPoint)
        {
            CheckStopPoint();
        }
        else if (!crossingStarted)
        {
            CheckCrossingLight();
        }
        else if (!finished)
        {
            CheckFinalDestination();
        }

        UpdateAnimation();
    }

    private void StartRoute()
    {
        reachedStopPoint = false;
        crossingStarted = false;
        finished = false;
        respawning = false;

        agent.isStopped = false;
        agent.speed = walkSpeed;

        agent.SetDestination(
            stopPoint.position
        );
    }

    private void CheckStopPoint()
    {
        if (agent.pathPending)
            return;

        float distance =
            Vector3.Distance(
                transform.position,
                stopPoint.position
            );

        if (distance <= stopPointCheckDistance)
        {
            reachedStopPoint = true;

            agent.isStopped = true;
            agent.ResetPath();

            Debug.Log(
                gameObject.name +
                " is waiting at the crossing."
            );
        }
    }

    private void CheckCrossingLight()
    {
        bool canCross =
            crossingGreenLight != null &&
            crossingGreenLight.enabled;

        // RED LIGHT
        if (!canCross)
        {
            agent.isStopped = true;
            return;
        }

        // GREEN LIGHT
        Transform destination =
            ChooseDestination();

        if (destination == null)
        {
            Debug.LogWarning(
                gameObject.name +
                " has no crossing destination."
            );

            return;
        }

        crossingStarted = true;

        agent.isStopped = false;

        agent.SetDestination(
            destination.position
        );

        Debug.Log(
            gameObject.name +
            " is crossing toward " +
            destination.name
        );
    }

    private Transform ChooseDestination()
    {
        switch (crossingDirection)
        {
            case CrossingDirection.Straight:
                return straightDestination;

            case CrossingDirection.Right:
                return rightDestination;

            case CrossingDirection.Random:

                if (straightDestination != null &&
                    rightDestination != null)
                {
                    if (Random.value < 0.5f)
                        return straightDestination;

                    return rightDestination;
                }

                if (straightDestination != null)
                    return straightDestination;

                return rightDestination;
        }

        return null;
    }

    private void CheckFinalDestination()
    {
        if (agent.pathPending)
            return;

        if (agent.remainingDistance <=
            agent.stoppingDistance + 0.15f)
        {
            if (!agent.hasPath ||
                agent.velocity.sqrMagnitude < 0.01f)
            {
                finished = true;

                agent.isStopped = true;

                Debug.Log(
                    gameObject.name +
                    " finished crossing."
                );

                StartCoroutine(
                    RespawnPedestrian()
                );
            }
        }
    }

    private IEnumerator RespawnPedestrian()
    {
        respawning = true;

        yield return new WaitForSeconds(
            respawnDelay
        );

        if (respawnPoint == null)
        {
            Debug.LogWarning(
                gameObject.name +
                " has no Respawn Point."
            );

            respawning = false;
            yield break;
        }

        agent.ResetPath();

        bool warped =
            agent.Warp(
                respawnPoint.position
            );

        if (!warped)
        {
            Debug.LogError(
                gameObject.name +
                " Respawn Point is not on the NavMesh."
            );

            respawning = false;
            yield break;
        }

        transform.rotation =
            respawnPoint.rotation;

        StartRoute();

        Debug.Log(
            gameObject.name +
            " respawned."
        );
    }

    private void UpdateAnimation()
    {
        if (animator == null)
            return;

        bool isWalking =
            !agent.isStopped &&
            agent.velocity.magnitude >
            walkingAnimationThreshold;

        animator.SetBool(
            "isWalking",
            isWalking
        );
    }
}