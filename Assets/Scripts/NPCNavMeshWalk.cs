using UnityEngine;
using UnityEngine.AI;

public class NPCNavMeshWalk : MonoBehaviour
{
    [Header("Targets")]
    public Transform[] targetPoints;

    [Header("References")]
    public NavMeshAgent agent;
    public Animator animator;

    [Header("Animation")]
    public float walkingThreshold = 0.1f;

    [Header("Debug")]
    public int currentTargetIndex = 0;

    // NPC is waiting for the ONE condition
    public bool waitingForCondition = false;

    // Once YES is given, NPC continues automatically
    private bool conditionPassed = false;

    private bool finished = false;

    void Start()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

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

        if (targetPoints == null || targetPoints.Length == 0)
        {
            Debug.LogError("No target points assigned!");
            return;
        }

        // Start walking to Target 1
        MoveToCurrentTarget();
    }

    void Update()
    {
        CheckIfReachedTarget();
        UpdateAnimation();
    }

    void MoveToCurrentTarget()
    {
        if (currentTargetIndex >= targetPoints.Length)
            return;

        if (targetPoints[currentTargetIndex] == null)
            return;

        agent.isStopped = false;

        agent.SetDestination(
            targetPoints[currentTargetIndex].position
        );

        Debug.Log(
            "Walking to Target " + (currentTargetIndex + 1)
        );
    }

    void CheckIfReachedTarget()
    {
        if (finished)
            return;

        if (waitingForCondition)
            return;

        if (agent.pathPending)
            return;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath ||
                agent.velocity.sqrMagnitude < 0.01f)
            {
                ReachedTarget();
            }
        }
    }

    void ReachedTarget()
    {
        agent.isStopped = true;

        Debug.Log(
            "Reached Target " + (currentTargetIndex + 1)
        );

        // ==========================
        // TARGET 1
        // Wait for the ONE condition
        // ==========================

        if (currentTargetIndex == 0 && !conditionPassed)
        {
            waitingForCondition = true;

            Debug.Log("Waiting for condition...");

            return;
        }

        // ==========================
        // FINAL TARGET
        // ==========================

        if (currentTargetIndex >= targetPoints.Length - 1)
        {
            finished = true;

            Debug.Log("NPC finished walking.");

            return;
        }

        // ==========================
        // Continue automatically
        // ==========================

        currentTargetIndex++;

        MoveToCurrentTarget();
    }

    // ==========================
    // CONDITION = YES
    // ==========================

    public void ConditionYes()
    {
        if (!waitingForCondition)
            return;

        Debug.Log("Condition YES - NPC continues");

        conditionPassed = true;

        waitingForCondition = false;

        currentTargetIndex++;

        MoveToCurrentTarget();
    }

    // ==========================
    // CONDITION = NO
    // ==========================

    public void ConditionNo()
    {
        if (!waitingForCondition)
            return;

        Debug.Log("Condition NO - NPC stays at Target 1");

        agent.isStopped = true;

        // NPC remains at Target 1
    }

    void UpdateAnimation()
    {
        if (animator == null || agent == null)
            return;

        bool isWalking =
            !agent.isStopped &&
            agent.velocity.magnitude > walkingThreshold;

        animator.SetBool("isWalking", isWalking);
    }
}