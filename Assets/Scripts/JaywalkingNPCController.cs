using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class JaywalkingNPCController : MonoBehaviour
{
    [Header("NPC")]
    public NavMeshAgent agent;
    public Animator animator;
    public NPCViolationOutline violationOutline;

    [Header("Jaywalk Route")]
    [Tooltip("Where the jaywalking NPC is trying to reach across the road.")]
    public Transform jaywalkDestination;

    [Header("Warning / Slow Walk")]
    public float normalWalkSpeed = 2f;
    public float slowWalkSpeed = 0.65f;
    [Range(0.05f, 1f)]
    public float slowAnimationSpeed = 0.45f;

    [Tooltip("How long the player has to stop the NPC after it enters the jaywalk trigger.")]
    public float interactionTimeLimit = 4f;

    [Header("Camera Focus After Successful Stop")]
    public Camera playerCamera;
    public Transform cameraFocusPoint;
    public float cameraFocusDuration = 0.65f;

    [Tooltip("Drag player movement / mouse-look scripts here so they are disabled during the conversation.")]
    public Behaviour[] playerControlScripts;

    [Header("Failure Scene")]
    [Tooltip("Scene name to load if the player does not stop the jaywalker in time.")]
    public string accidentSceneName = "CarAccident";

    [Header("Events")]
    [Tooltip("Hook your existing NPC dialogue StartDialogue/Talk method here.")]
    public UnityEvent onStoppedInTime;

    [Tooltip("Optional event just before the accident scene loads.")]
    public UnityEvent onFailedToStop;

    public bool WarningActive { get; private set; }
    public bool WasStopped { get; private set; }

    private float warningTimer;
    private bool failed;

    private Vector3 cameraStartLocalPosition;
    private Quaternion cameraStartLocalRotation;
    private bool cameraStartSaved;

    private void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (violationOutline == null)
            violationOutline = GetComponent<NPCViolationOutline>();

        if (agent == null)
        {
            Debug.LogError(gameObject.name + " has no NavMeshAgent.");
            enabled = false;
            return;
        }

        agent.speed = normalWalkSpeed;

        if (jaywalkDestination != null && agent.isOnNavMesh)
            agent.SetDestination(jaywalkDestination.position);

        if (playerCamera != null)
        {
            cameraStartLocalPosition = playerCamera.transform.localPosition;
            cameraStartLocalRotation = playerCamera.transform.localRotation;
            cameraStartSaved = true;
        }

        if (violationOutline != null)
            violationOutline.StopViolationWarning();
    }

    private void Update()
    {
        if (agent == null || !agent.isOnNavMesh)
            return;

        if (WarningActive && !WasStopped && !failed)
        {
            warningTimer += Time.deltaTime;

            if (warningTimer >= interactionTimeLimit)
                FailJaywalkEvent();
        }

        UpdateAnimation();
    }

    public void BeginJaywalkWarning()
    {
        if (WarningActive || WasStopped || failed)
            return;

        WarningActive = true;
        warningTimer = 0f;

        agent.speed = slowWalkSpeed;

        if (animator != null)
            animator.speed = slowAnimationSpeed;

        if (violationOutline != null)
            violationOutline.AboutToViolateRule();

        Debug.Log(gameObject.name + " is attempting to jaywalk. Player can stop them now.");
    }

    // Call this from your existing PlayerInteract script when E is pressed on this NPC.
    public void StopJaywalkerInTime()
    {
        if (!WarningActive || WasStopped || failed)
            return;

        WasStopped = true;
        WarningActive = false;

        agent.isStopped = true;
        agent.ResetPath();

        if (animator != null)
        {
            animator.speed = 1f;
            animator.SetBool("isWalking", false);
        }

        if (violationOutline != null)
            violationOutline.StopViolationWarning();

        SetPlayerControls(false);

        if (playerCamera != null && cameraFocusPoint != null)
            StartCoroutine(FocusCameraThenTalk());
        else
            onStoppedInTime?.Invoke();

        Debug.Log(gameObject.name + " was stopped before jaywalking.");
    }

    private IEnumerator FocusCameraThenTalk()
    {
        Transform cam = playerCamera.transform;

        Vector3 startPosition = cam.position;
        Quaternion startRotation = cam.rotation;

        float elapsed = 0f;

        while (elapsed < cameraFocusDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / cameraFocusDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            cam.position = Vector3.Lerp(
                startPosition,
                cameraFocusPoint.position,
                t
            );

            cam.rotation = Quaternion.Slerp(
                startRotation,
                cameraFocusPoint.rotation,
                t
            );

            yield return null;
        }

        cam.position = cameraFocusPoint.position;
        cam.rotation = cameraFocusPoint.rotation;

        // Your existing dialogue can begin here.
        onStoppedInTime?.Invoke();
    }

    // Call this from your dialogue system when the conversation is finished.
    public void EndConversation()
    {
        if (playerCamera != null && cameraStartSaved)
        {
            playerCamera.transform.localPosition = cameraStartLocalPosition;
            playerCamera.transform.localRotation = cameraStartLocalRotation;
        }

        SetPlayerControls(true);
    }

    private void FailJaywalkEvent()
    {
        if (failed || WasStopped)
            return;

        failed = true;
        WarningActive = false;

        if (violationOutline != null)
            violationOutline.StopViolationWarning();

        onFailedToStop?.Invoke();

        if (string.IsNullOrWhiteSpace(accidentSceneName))
        {
            Debug.LogError("Accident Scene Name is empty.");
            return;
        }

        Debug.Log("Player failed to stop the jaywalker. Loading scene: " + accidentSceneName);
        SceneManager.LoadScene(accidentSceneName);
    }

    private void UpdateAnimation()
    {
        if (animator == null)
            return;

        bool isWalking =
            !agent.isStopped &&
            agent.velocity.sqrMagnitude > 0.01f;

        animator.SetBool("isWalking", isWalking);
    }

    private void SetPlayerControls(bool enabledState)
    {
        if (playerControlScripts == null)
            return;

        foreach (Behaviour script in playerControlScripts)
        {
            if (script != null)
                script.enabled = enabledState;
        }
    }
}
