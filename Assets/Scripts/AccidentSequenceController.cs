using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class AccidentSequenceController : MonoBehaviour
{
    [Header("Crash Car")]
    [Tooltip("The car that will drive into the pedestrian.")]
    public Transform crashCar;

    [Header("Victim Setup")]
    [Tooltip("Turn this ON when the victim is parented to a bike/vehicle and should detach on impact.")]
    public bool detachVictimOnImpact = false;

    [Tooltip("Where the car should drive to before the impact happens.")]
    public Transform impactPoint;

    [Tooltip("Optional point the car continues toward after the impact.")]
    public Transform carEndPoint;

    public float carSpeed = 8f;
    public float impactDistance = 0.8f;

    [Header("Crash Audio")]
    public AudioSource crashAudioSource;
    public AudioClip crashSound;

    [Header("Victim")]
    [Tooltip("The jaywalking NPC that gets hit.")]
    public GameObject victim;

    [Tooltip("Rigidbody on the ROOT of the victim.")]
    public Rigidbody victimRigidbody;

    [Tooltip("Animator on the victim.")]
    public Animator victimAnimator;

    [Tooltip("Optional NavMeshAgent on the victim.")]
    public NavMeshAgent victimAgent;

    [Header("Motorcycle Crash Physics")]
    [Tooltip("Optional. Assign the motorcycle/bike Rigidbody only for motorcycle accident scenes. Leave None for other NPC accidents.")]
    public Rigidbody motorcycleRigidbody;

    [Tooltip("Forward push applied to the motorcycle when the crash happens.")]
    public float motorcyclePushForce = 5f;

    [Tooltip("Small upward force applied to the motorcycle when the crash happens.")]
    public float motorcycleUpwardForce = 1.5f;

    [Tooltip("Rotational force that makes the motorcycle tip/fall over.")]
    public float motorcycleSpinForce = 6f;

    [Header("Flying Effect")]
    public float flyForce = 12f;
    public float upwardForce = 7f;
    public float spinForce = 7f;

    [Header("Accident Ending")]
    [Tooltip("How long after the crash before the blackout begins.")]
    public float blackoutDelay = 2.5f;

    [Tooltip("How long the screen takes to fade to black.")]
    public float blackFadeDuration = 1.5f;

    [Tooltip("How long the safety message takes to fade in/out.")]
    public float messageFadeDuration = 1f;

    [Tooltip("How long the safety message stays visible.")]
    public float messageHoldDuration = 3f;

    [Header("Black Screen")]
    [Tooltip("Full-screen black Image with a CanvasGroup.")]
    public CanvasGroup blackScreen;

    [Header("Safety Message")]
    [Tooltip("CanvasGroup on the safety message text.")]
    public CanvasGroup safetyMessageGroup;

    [Tooltip("TextMeshPro text used for the safety message.")]
    public TextMeshProUGUI safetyMessageText;

    [TextArea(2, 4)]
    public string safetyMessage =
        "You only get one life. Stay alert, stay safe, and never take the road for granted.";

    [Header("Scene")]
    public string day1SceneName = "Day1";

    private bool impactTriggered = false;
    private bool sequenceFinished = false;

    private void Awake()
    {
        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);
            blackScreen.alpha = 0f;
            blackScreen.interactable = false;
            blackScreen.blocksRaycasts = false;
        }

        if (safetyMessageGroup != null)
        {
            safetyMessageGroup.gameObject.SetActive(true);
            safetyMessageGroup.alpha = 0f;
            safetyMessageGroup.interactable = false;
            safetyMessageGroup.blocksRaycasts = false;
        }

        if (safetyMessageText != null)
        {
            safetyMessageText.text = safetyMessage;
        }
    }

    private void Start()
    {
        if (victimRigidbody != null)
        {
            victimRigidbody.isKinematic = true;
            victimRigidbody.useGravity = false;
        }

        // Keep the motorcycle controlled by NavMesh/traffic before impact.
        // This only runs when a motorcycle Rigidbody has been assigned.
        if (motorcycleRigidbody != null)
        {
            motorcycleRigidbody.isKinematic = true;
            motorcycleRigidbody.useGravity = false;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (sequenceFinished)
            return;

        if (!impactTriggered)
        {
            MoveCarTowardImpact();
        }
        else
        {
            MoveCarAfterImpact();
        }
    }

    private void MoveCarTowardImpact()
    {
        if (crashCar == null || impactPoint == null)
            return;

        Vector3 direction =
            impactPoint.position - crashCar.position;

        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            crashCar.rotation = Quaternion.Slerp(
                crashCar.rotation,
                Quaternion.LookRotation(direction.normalized),
                8f * Time.deltaTime
            );
        }

        crashCar.position = Vector3.MoveTowards(
            crashCar.position,
            impactPoint.position,
            carSpeed * Time.deltaTime
        );

        float distance = Vector3.Distance(
            crashCar.position,
            impactPoint.position
        );

        if (distance <= impactDistance)
        {
            TriggerImpact();
        }
    }

    private void TriggerImpact()
    {
        if (impactTriggered)
            return;

        impactTriggered = true;

        // Play crash sound.
        if (crashAudioSource != null && crashSound != null)
        {
            crashAudioSource.PlayOneShot(crashSound);
        }

        // Stop NavMesh movement.
        if (victimAgent != null)
        {
            if (victimAgent.isOnNavMesh)
            {
                victimAgent.isStopped = true;
                victimAgent.ResetPath();
            }

            victimAgent.enabled = false;
        }

        // Stop animation so physics controls the NPC.
        if (victimAnimator != null)
        {
            victimAnimator.enabled = false;
        }

        // OPTIONAL:
        // Detach the victim from a bike/vehicle before physics takes over.
        // Leave Detach Victim On Impact OFF for normal NPCs.
        if (detachVictimOnImpact && victim != null)
        {
            victim.transform.SetParent(null, true);
        }

        // Launch the victim.
        if (victimRigidbody != null)
        {
            victimRigidbody.isKinematic = false;
            victimRigidbody.useGravity = true;
            victimRigidbody.constraints = RigidbodyConstraints.None;

            victimRigidbody.linearVelocity = Vector3.zero;
            victimRigidbody.angularVelocity = Vector3.zero;

            Vector3 launchDirection =
                crashCar != null
                ? crashCar.forward
                : Vector3.forward;

            Vector3 launchForce =
                launchDirection * flyForce +
                Vector3.up * upwardForce;

            victimRigidbody.AddForce(
                launchForce,
                ForceMode.Impulse
            );

            victimRigidbody.AddTorque(
                new Vector3(
                    spinForce,
                    spinForce * 0.5f,
                    spinForce
                ),
                ForceMode.Impulse
            );
        }

        // OPTIONAL MOTORCYCLE CRASH PHYSICS:
        // Leave Motorcycle Rigidbody as None for non-motorcycle accident scenes.
        if (motorcycleRigidbody != null)
        {
            motorcycleRigidbody.isKinematic = false;
            motorcycleRigidbody.useGravity = true;
            motorcycleRigidbody.constraints = RigidbodyConstraints.None;

            motorcycleRigidbody.linearVelocity = Vector3.zero;
            motorcycleRigidbody.angularVelocity = Vector3.zero;

            Vector3 motorcycleDirection =
                crashCar != null
                ? crashCar.forward
                : Vector3.forward;

            Vector3 motorcycleForce =
                motorcycleDirection * motorcyclePushForce +
                Vector3.up * motorcycleUpwardForce;

            motorcycleRigidbody.AddForce(
                motorcycleForce,
                ForceMode.Impulse
            );

            // Apply sideways/forward rotation so the bike tips over
            // instead of staying perfectly upright.
            motorcycleRigidbody.AddTorque(
                new Vector3(
                    motorcycleSpinForce,
                    motorcycleSpinForce * 0.35f,
                    motorcycleSpinForce
                ),
                ForceMode.Impulse
            );
        }

        StartCoroutine(AccidentEndingSequence());
    }

    private void MoveCarAfterImpact()
    {
        if (crashCar == null || carEndPoint == null)
            return;

        crashCar.position = Vector3.MoveTowards(
            crashCar.position,
            carEndPoint.position,
            carSpeed * Time.deltaTime
        );
    }

    private IEnumerator AccidentEndingSequence()
    {
        // Let the player see the crash first.
        yield return new WaitForSeconds(blackoutDelay);

        sequenceFinished = true;

        // If no black screen is assigned, return directly to Day1.
        if (blackScreen == null)
        {
            Debug.LogWarning(
                "Black Screen is not assigned. Returning to Day1 without fade."
            );

            SceneManager.LoadScene(day1SceneName);
            yield break;
        }

        blackScreen.blocksRaycasts = true;

        // Fade screen to black.
        yield return FadeCanvasGroup(
            blackScreen,
            0f,
            1f,
            blackFadeDuration
        );

        // Set the message in case it was changed in the Inspector.
        if (safetyMessageText != null)
        {
            safetyMessageText.text = safetyMessage;
        }

        // Fade safety message in.
        if (safetyMessageGroup != null)
        {
            yield return FadeCanvasGroup(
                safetyMessageGroup,
                0f,
                1f,
                messageFadeDuration
            );

            // Keep message visible.
            yield return new WaitForSecondsRealtime(
                messageHoldDuration
            );

            // Fade safety message out.
            yield return FadeCanvasGroup(
                safetyMessageGroup,
                1f,
                0f,
                messageFadeDuration
            );
        }
        else
        {
            // Still wait briefly if no text group was assigned.
            yield return new WaitForSecondsRealtime(
                messageHoldDuration
            );
        }

        Time.timeScale = 1f;

        // Screen remains black while Day1 loads.
        yield return SceneManager.LoadSceneAsync(day1SceneName);
    }

    private IEnumerator FadeCanvasGroup(
        CanvasGroup canvasGroup,
        float startAlpha,
        float endAlpha,
        float duration)
    {
        if (canvasGroup == null)
            yield break;

        if (duration <= 0f)
        {
            canvasGroup.alpha = endAlpha;
            yield break;
        }

        float timer = 0f;
        canvasGroup.alpha = startAlpha;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            canvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                endAlpha,
                timer / duration
            );

            yield return null;
        }

        canvasGroup.alpha = endAlpha;
    }
}