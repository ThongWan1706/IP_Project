using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GroupAccidentSequenceController : MonoBehaviour
{
    [System.Serializable]
    public class AccidentVictim
    {
        public GameObject victim;
        public Rigidbody rigidbody;
        public Animator animator;
        public NavMeshAgent agent;
    }

    [Header("Crash Car")]
    public Transform crashCar;
    public Transform impactPoint;
    public Transform carEndPoint;
    public float carSpeed = 8f;
    public float impactDistance = 0.8f;

    [Header("Crash Audio")]
    public AudioSource crashAudioSource;
    public AudioClip crashSound;

    [Header("Victim Group")]
    [Tooltip("Set Size to 4 and assign the four NPCs here.")]
    public AccidentVictim[] victims = new AccidentVictim[4];

    [Header("Flying / Falling Effect")]
    public float flyForce = 12f;
    public float upwardForce = 7f;
    public float spinForce = 7f;
    public float groupSpreadForce = 2.5f;

    [Header("Accident Ending")]
    public float blackoutDelay = 2.5f;
    public float blackFadeDuration = 1.5f;
    public float messageFadeDuration = 1f;
    public float messageHoldDuration = 3f;

    [Header("Black Screen")]
    public CanvasGroup blackScreen;

    [Header("Safety Message")]
    public CanvasGroup safetyMessageGroup;
    public TextMeshProUGUI safetyMessageText;

    [TextArea(2, 4)]
    public string safetyMessage =
        "You only get one life. Stay alert, stay safe, and never take the road for granted.";

    [Header("Scene")]
    public string returnSceneName = "Day2";

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
            safetyMessageText.text = safetyMessage;
    }

    private void Start()
    {
        PrepareVictimsForImpact();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void PrepareVictimsForImpact()
    {
        if (victims == null)
            return;

        for (int i = 0; i < victims.Length; i++)
        {
            AccidentVictim entry = victims[i];

            if (entry == null || entry.victim == null)
                continue;

            if (entry.rigidbody == null)
                entry.rigidbody = entry.victim.GetComponent<Rigidbody>();

            if (entry.animator == null)
                entry.animator = entry.victim.GetComponentInChildren<Animator>();

            if (entry.agent == null)
                entry.agent = entry.victim.GetComponent<NavMeshAgent>();

            if (entry.rigidbody != null)
            {
                entry.rigidbody.isKinematic = true;
                entry.rigidbody.useGravity = false;
            }
            else
            {
                Debug.LogWarning(
                    entry.victim.name +
                    " has no Rigidbody on its root, so it cannot fall with physics."
                );
            }
        }
    }

    private void Update()
    {
        if (sequenceFinished)
            return;

        if (!impactTriggered)
            MoveCarTowardImpact();
        else
            MoveCarAfterImpact();
    }

    private void MoveCarTowardImpact()
    {
        if (crashCar == null || impactPoint == null)
            return;

        Vector3 direction = impactPoint.position - crashCar.position;
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
            TriggerImpact();
    }

    private void TriggerImpact()
    {
        if (impactTriggered)
            return;

        impactTriggered = true;

        if (crashAudioSource != null && crashSound != null)
            crashAudioSource.PlayOneShot(crashSound);

        LaunchAllVictims();

        StartCoroutine(AccidentEndingSequence());
    }

    private void LaunchAllVictims()
    {
        if (victims == null || victims.Length == 0)
        {
            Debug.LogWarning("No accident victims have been assigned.");
            return;
        }

        for (int i = 0; i < victims.Length; i++)
        {
            AccidentVictim entry = victims[i];

            if (entry == null || entry.victim == null)
                continue;

            if (entry.agent != null)
            {
                if (entry.agent.enabled && entry.agent.isOnNavMesh)
                {
                    entry.agent.isStopped = true;
                    entry.agent.ResetPath();
                }

                entry.agent.enabled = false;
            }

            if (entry.animator != null)
                entry.animator.enabled = false;

            if (entry.rigidbody == null)
                continue;

            entry.rigidbody.isKinematic = false;
            entry.rigidbody.useGravity = true;

            Vector3 launchDirection =
                crashCar != null ? crashCar.forward : Vector3.forward;

            float centerIndex = (victims.Length - 1) * 0.5f;
            float spreadAmount = i - centerIndex;

            Vector3 sideways =
                crashCar != null
                ? crashCar.right * spreadAmount * groupSpreadForce
                : Vector3.right * spreadAmount * groupSpreadForce;

            Vector3 launchForce =
                launchDirection * flyForce +
                Vector3.up * upwardForce +
                sideways;

            entry.rigidbody.AddForce(
                launchForce,
                ForceMode.Impulse
            );

            float spinDirection = (i % 2 == 0) ? 1f : -1f;

            entry.rigidbody.AddTorque(
                new Vector3(
                    spinForce * spinDirection,
                    spinForce * 0.5f,
                    spinForce * -spinDirection
                ),
                ForceMode.Impulse
            );
        }

        Debug.Log("Group accident impact triggered for " + victims.Length + " victims.");
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
        yield return new WaitForSeconds(blackoutDelay);

        sequenceFinished = true;

        if (blackScreen == null)
        {
            Debug.LogWarning(
                "Black Screen is not assigned. Returning without fade."
            );

            SceneManager.LoadScene(returnSceneName);
            yield break;
        }

        blackScreen.blocksRaycasts = true;

        yield return FadeCanvasGroup(
            blackScreen,
            0f,
            1f,
            blackFadeDuration
        );

        if (safetyMessageText != null)
            safetyMessageText.text = safetyMessage;

        if (safetyMessageGroup != null)
        {
            yield return FadeCanvasGroup(
                safetyMessageGroup,
                0f,
                1f,
                messageFadeDuration
            );

            yield return new WaitForSecondsRealtime(
                messageHoldDuration
            );

            yield return FadeCanvasGroup(
                safetyMessageGroup,
                1f,
                0f,
                messageFadeDuration
            );
        }
        else
        {
            yield return new WaitForSecondsRealtime(
                messageHoldDuration
            );
        }

        Time.timeScale = 1f;

        yield return SceneManager.LoadSceneAsync(returnSceneName);
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