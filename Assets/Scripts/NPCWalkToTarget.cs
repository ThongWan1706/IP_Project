using UnityEngine;

public class NPCWalkToTarget : MonoBehaviour
{
    [Header("Target")]
    public Transform targetPoint;

    [Header("Movement")]
    public float moveSpeed = 1.5f;
    public float rotationSpeed = 5f;

    [Tooltip("How close the NPC must be before stopping.")]
    public float stoppingDistance = 0.3f;

    [Header("Animation")]
    public Animator animator;

    private bool isWalking = false;

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (animator == null)
        {
            Debug.LogError("Animator not found!");
        }
    }

    void Update()
    {
        MoveNPC();
        UpdateAnimation();
    }

    void MoveNPC()
    {
        // No target = don't walk
        if (targetPoint == null)
        {
            isWalking = false;
            return;
        }

        Vector3 direction =
            targetPoint.position - transform.position;

        // Ignore height difference
        direction.y = 0f;

        float distance = direction.magnitude;

        // ==================================
        // WALK
        // ==================================
        if (distance > stoppingDistance)
        {
            isWalking = true;

            direction.Normalize();

            // Face target
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation =
                    Quaternion.LookRotation(direction);

                transform.rotation =
                    Quaternion.Slerp(
                        transform.rotation,
                        targetRotation,
                        rotationSpeed * Time.deltaTime
                    );
            }

            // Move toward target
            transform.position +=
                direction *
                moveSpeed *
                Time.deltaTime;
        }

        // ==================================
        // STOP
        // ==================================
        else
        {
            isWalking = false;

            Debug.Log("NPC reached target and stopped.");
        }
    }

    void UpdateAnimation()
    {
        if (animator == null)
            return;

        animator.SetBool("isWalking", isWalking);
    }
}