using UnityEngine;
using UnityEngine.AI;

public class ChildrenGroupController : MonoBehaviour
{
    [Header("Children In This Group")]
    [Tooltip("Drag the ROOT object of each child NPC here.")]
    public Transform[] children;

    [Header("Scripts To Pause")]
    [Tooltip("These can be filled automatically from the Children list.")]
    public JaywalkingNPCController[] movementScripts;

    [Header("Look At Player")]
    [Tooltip("Usually MainCamera or an empty object attached to the player camera.")]
    public Transform playerFocusPoint;

    public float rotationSpeed = 5f;


    // =========================================================
    // CHILDREN LAUGHING AUDIO
    // =========================================================

    [Header("Children Laughing Audio")]
    [Tooltip("AudioSource on the ChildrenGroupController object.")]
    public AudioSource laughingAudioSource;

    [Tooltip("The laughing sound of the children.")]
    public AudioClip laughingSound;

    [Tooltip("How fast the children need to be moving before laughter plays.")]
    public float movementSoundThreshold = 0.1f;


    private bool groupStopped = false;

    private NavMeshAgent[] agents;
    private Animator[] animators;


    void Start()
    {
        SetupChildren();

        SetupLaughingAudio();
    }


    void SetupChildren()
    {
        if (children == null || children.Length == 0)
        {
            Debug.LogWarning(
                "ChildrenGroupController: No children assigned."
            );

            return;
        }

        agents = new NavMeshAgent[children.Length];
        animators = new Animator[children.Length];


        if (movementScripts == null ||
            movementScripts.Length != children.Length)
        {
            movementScripts =
                new JaywalkingNPCController[children.Length];
        }


        for (int i = 0; i < children.Length; i++)
        {
            if (children[i] == null)
                continue;


            // Find NavMeshAgent
            agents[i] =
                children[i].GetComponentInChildren<NavMeshAgent>();


            // Find Animator
            animators[i] =
                children[i].GetComponentInChildren<Animator>();


            // Find JaywalkingNPCController
            if (movementScripts[i] == null)
            {
                movementScripts[i] =
                    children[i].GetComponentInChildren
                    <JaywalkingNPCController>();
            }


            if (agents[i] == null)
            {
                Debug.LogWarning(
                    children[i].name +
                    " does not have a NavMeshAgent."
                );
            }


            if (animators[i] == null)
            {
                Debug.LogWarning(
                    children[i].name +
                    " does not have an Animator."
                );
            }


            if (movementScripts[i] == null)
            {
                Debug.LogWarning(
                    children[i].name +
                    " does not have a JaywalkingNPCController."
                );
            }
        }
    }


    // =========================================================
    // AUDIO SETUP
    // =========================================================

    void SetupLaughingAudio()
    {
        if (laughingAudioSource == null)
        {
            laughingAudioSource =
                GetComponent<AudioSource>();
        }


        if (laughingAudioSource != null)
        {
            laughingAudioSource.playOnAwake = false;

            // We want the laughing sound to continue
            // while the children are running.
            laughingAudioSource.loop = true;


            if (laughingSound != null)
            {
                laughingAudioSource.clip =
                    laughingSound;
            }
        }
    }


    void Update()
    {
        if (groupStopped)
        {
            StopLaughing();

            LookAtPlayer();

            return;
        }


        // Children have not been stopped yet.
        // Check if they are moving.
        UpdateLaughingSound();
    }


    // =========================================================
    // CHECK IF CHILDREN ARE RUNNING
    // =========================================================

    void UpdateLaughingSound()
    {
        if (laughingAudioSource == null ||
            laughingSound == null)
        {
            return;
        }


        bool someoneIsMoving = false;


        foreach (NavMeshAgent agent in agents)
        {
            if (agent == null)
                continue;


            if (!agent.isActiveAndEnabled)
                continue;


            if (!agent.isOnNavMesh)
                continue;


            // Check actual NPC movement
            if (agent.velocity.magnitude >
                movementSoundThreshold)
            {
                someoneIsMoving = true;

                break;
            }
        }


        // Start laughing when the group starts moving
        if (someoneIsMoving)
        {
            if (!laughingAudioSource.isPlaying)
            {
                laughingAudioSource.Play();
            }
        }
        else
        {
            // If they naturally stop moving,
            // stop the laughter too.
            StopLaughing();
        }
    }


    // =========================================================
    // STOP THE LAUGHING
    // =========================================================

    void StopLaughing()
    {
        if (laughingAudioSource != null &&
            laughingAudioSource.isPlaying)
        {
            laughingAudioSource.Stop();
        }
    }


    // =========================================================
    // PLAYER STOPS THE CHILDREN IN TIME
    // =========================================================

    public void StopGroup()
    {
        groupStopped = true;

        Debug.Log("Children group stopped.");


        // Immediately stop laughing
        StopLaughing();


        // Stop all NavMeshAgents
        foreach (NavMeshAgent agent in agents)
        {
            if (agent != null &&
                agent.isActiveAndEnabled &&
                agent.isOnNavMesh)
            {
                agent.isStopped = true;

                agent.velocity =
                    Vector3.zero;
            }
        }


        // Disable all Jaywalking scripts
        foreach (JaywalkingNPCController movementScript
                 in movementScripts)
        {
            if (movementScript != null)
            {
                movementScript.enabled = false;
            }
        }


        // Stop walking/running animation
        foreach (Animator animator in animators)
        {
            if (animator == null)
                continue;


            SetWalkingAnimation(
                animator,
                false
            );
        }
    }


    // =========================================================
    // RESUME GROUP
    // =========================================================

    public void ResumeGroup()
    {
        groupStopped = false;

        Debug.Log("Children group resumed.");


        // Turn walking scripts back on
        foreach (JaywalkingNPCController movementScript
                 in movementScripts)
        {
            if (movementScript != null)
            {
                movementScript.enabled = true;
            }
        }


        // Resume NavMeshAgents
        foreach (NavMeshAgent agent in agents)
        {
            if (agent != null &&
                agent.isActiveAndEnabled &&
                agent.isOnNavMesh)
            {
                agent.isStopped = false;
            }
        }

        // You don't need Play() here.
        // UpdateLaughingSound() will automatically
        // start the laughter once they begin moving.
    }


    // =========================================================
    // ALL CHILDREN LOOK AT PLAYER
    // =========================================================

    void LookAtPlayer()
    {
        if (playerFocusPoint == null)
            return;


        foreach (Transform child in children)
        {
            if (child == null)
                continue;


            Vector3 direction =
                playerFocusPoint.position -
                child.position;


            // Only rotate horizontally
            direction.y = 0f;


            if (direction.sqrMagnitude < 0.01f)
                continue;


            Quaternion targetRotation =
                Quaternion.LookRotation(direction);


            child.rotation =
                Quaternion.Slerp(
                    child.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
        }
    }


    // =========================================================
    // STOP WALKING/RUNNING ANIMATION
    // =========================================================

    void SetWalkingAnimation(
        Animator animator,
        bool walking)
    {
        foreach (AnimatorControllerParameter parameter
                 in animator.parameters)
        {
            // Animator using isWalking
            if (parameter.name == "isWalking" &&
                parameter.type ==
                AnimatorControllerParameterType.Bool)
            {
                animator.SetBool(
                    "isWalking",
                    walking
                );
            }


            // Animator using Speed
            if (parameter.name == "Speed" &&
                parameter.type ==
                AnimatorControllerParameterType.Float)
            {
                animator.SetFloat(
                    "Speed",
                    walking ? 1f : 0f
                );
            }
        }
    }
}