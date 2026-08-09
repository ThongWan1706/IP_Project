using UnityEngine;
using UnityEngine.AI;

public class NPCNavMeshWalk : MonoBehaviour
{
    [Header("Target")]
    public Transform targetPoint;

    [Header("References")]
    public NavMeshAgent agent;
    public Animator animator;

    [Header("Animation")]
    public float walkingThreshold = 0.1f;

    private bool hasReachedTarget = false;

    void Start()
    {
        // Find NavMesh Agent automatically
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        // Find Animator automatically
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent not found!");
            enabled = false;
            return;
        }

        if (animator == null)
        {
            Debug.LogError("Animator not found!");
            enabled = false;
            return;
        }

        // Give the NPC its destination
        if (targetPoint != null)
        {
            agent.SetDestination(targetPoint.position);
        }
    }

    void Update()
    {
        UpdateMovement();
        UpdateAnimation();
    }

    void UpdateMovement()
    {
        if (targetPoint == null)
            return;

        // Keep destination updated
        if (!hasReachedTarget)
        {
            agent.SetDestination(targetPoint.position);
        }

        // Wait until NavMesh finishes calculating the path
        if (agent.pathPending)
            return;

        // Check if NPC has reached the destination
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            // Make sure it has actually stopped moving
            if (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f)
            {
                hasReachedTarget = true;

                agent.isStopped = true;
            }
        }
    }

    void UpdateAnimation()
    {
        if (animator == null || agent == null)
            return;

        // Check how fast the NavMeshAgent is moving
        bool isWalking =
            agent.velocity.magnitude > walkingThreshold;

        animator.SetBool("isWalking", isWalking);
    }

    // Call this if you want to give the NPC a new target later
    public void SetNewTarget(Transform newTarget)
    {
        if (newTarget == null)
            return;

        targetPoint = newTarget;

        hasReachedTarget = false;

        agent.isStopped = false;

        agent.SetDestination(targetPoint.position);
    }
}