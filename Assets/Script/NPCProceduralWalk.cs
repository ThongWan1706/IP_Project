using UnityEngine;

public class NPCProceduralWalk : MonoBehaviour
{
    [Header("Target")]
    public Transform targetPoint;

    [Header("Movement")]
    public float moveSpeed = 1.5f;
    public float rotationSpeed = 5f;
    public float stoppingDistance = 0.2f;

    [Header("Animation")]
    public Animator animator;

    private bool isWalking;

    void Start()
    {
        // Automatically find Animator if not assigned
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (animator == null)
        {
            Debug.LogError("Animator not found on NPC!");
        }
    }

    void Update()
    {
        MoveNPC();
        UpdateAnimation();
    }

    void MoveNPC()
    {
        if (targetPoint == null)
        {
            isWalking = false;
            return;
        }

        Vector3 direction =
            targetPoint.position - transform.position;

        // Keep NPC upright
        direction.y = 0f;

        float distance = direction.magnitude;

        if (distance > stoppingDistance)
        {
            isWalking = true;

            direction.Normalize();

            // Rotate toward destination
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

            // Move NPC
            transform.position +=
                direction *
                moveSpeed *
                Time.deltaTime;
        }
        else
        {
            isWalking = false;
        }
    }

    void UpdateAnimation()
    {
        if (animator == null)
            return;

        animator.SetBool("isWalking", isWalking);
    }   
}
