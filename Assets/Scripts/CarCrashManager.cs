using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class CarCrashManager : MonoBehaviour
{
    [Header("Car References")]
    [SerializeField] private GameObject carA;
    [SerializeField] private GameObject carB;

    [Header("Crash Forces")]
    [SerializeField] private float inwardForce = 15f;
    [SerializeField] private float upwardLiftForce = 5f;
    [SerializeField] private float rotationalTorque = 10f;

    [Header("Crash Sound")]
    [SerializeField] private AudioSource crashAudioSource;
    [SerializeField] private AudioClip crashSound;

    [Header("Day Intro")]
    [Tooltip("Crash cannot happen until NotifyDayIntroFinished() is called.")]
    [SerializeField] private bool waitForDayIntro = true;

    [Header("Scene Transition")]
    [SerializeField] private float delayBeforeSceneLoad = 3f;
    [SerializeField] private int sceneToLoadIndex = 6;

    [Header("Black Screen")]
    [SerializeField] private CanvasGroup blackScreen;
    [SerializeField] private float fadeDuration = 1.5f;

    private bool introFinished = false;
    private bool crashRequested = false;
    private bool crashTriggered = false;

    private void Awake()
    {
        introFinished = !waitForDayIntro;

        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);
            blackScreen.alpha = 0f;
            blackScreen.blocksRaycasts = false;
            blackScreen.interactable = false;
        }

        // VERY IMPORTANT:
        // Make sure crash sound cannot play on scene start.
        if (crashAudioSource != null)
        {
            crashAudioSource.playOnAwake = false;
            crashAudioSource.Stop();
        }
    }

    // ========================================================
    // SOMETHING TRIED TO START THE CRASH
    // ========================================================

    public void TriggerCrashSequence()
    {
        if (crashTriggered)
            return;

        // Day Intro still showing.
        if (waitForDayIntro && !introFinished)
        {
            crashRequested = true;

            Debug.Log(
                "Crash requested, but waiting for Day Intro to finish."
            );

            return;
        }

        StartCrash();
    }

    // ========================================================
    // CALL THIS AFTER THE DAY INTRO COMPLETELY DISAPPEARS
    // ========================================================

    public void NotifyDayIntroFinished()
    {
        if (introFinished)
            return;

        introFinished = true;

        Debug.Log("Day Intro finished. Crash system enabled.");

        // If the crash was already triggered while the intro was showing,
        // start it only now.
        if (crashRequested)
        {
            crashRequested = false;
            StartCrash();
        }
    }

    // ========================================================
    // ACTUAL CRASH
    // ========================================================

    private void StartCrash()
    {
        if (crashTriggered)
            return;

        if (!introFinished && waitForDayIntro)
            return;

        if (carA == null || carB == null)
        {
            Debug.LogError(
                "CarCrashManager: Car A or Car B is missing."
            );

            return;
        }

        crashTriggered = true;

        Debug.Log("CAR CRASH STARTED");

        // Play crash sound ONLY NOW.
        if (crashAudioSource != null &&
            crashSound != null)
        {
            crashAudioSource.PlayOneShot(crashSound);
        }

        PrepareCarForPhysics(
            carA,
            carB.transform.position
        );

        PrepareCarForPhysics(
            carB,
            carA.transform.position
        );

        StartCoroutine(CrashThenFadeAndLoad());
    }

    private void PrepareCarForPhysics(
        GameObject car,
        Vector3 targetPosition)
    {
        if (car == null)
            return;

        // These are disabled ONLY when the actual crash starts.
        CarNavMeshTraffic traffic =
            car.GetComponentInChildren<CarNavMeshTraffic>();

        if (traffic != null)
        {
            traffic.enabled = false;
        }

        NavMeshAgent agent =
            car.GetComponentInChildren<NavMeshAgent>();

        if (agent != null)
        {
            if (agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }

            agent.enabled = false;
        }

        Rigidbody rb = car.GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = car.AddComponent<Rigidbody>();
        }

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 direction =
            targetPosition - car.transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            direction.Normalize();
        }

        Vector3 force =
            direction * inwardForce +
            Vector3.up * upwardLiftForce;

        rb.AddForce(
            force,
            ForceMode.VelocityChange
        );

        Vector3 torque = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ) * rotationalTorque;

        rb.AddTorque(
            torque,
            ForceMode.VelocityChange
        );
    }

    private IEnumerator CrashThenFadeAndLoad()
    {
        yield return new WaitForSecondsRealtime(
            delayBeforeSceneLoad
        );

        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);
            blackScreen.blocksRaycasts = true;
            blackScreen.interactable = true;

            float timer = 0f;

            while (timer < fadeDuration)
            {
                timer += Time.unscaledDeltaTime;

                blackScreen.alpha = Mathf.Lerp(
                    0f,
                    1f,
                    timer / fadeDuration
                );

                yield return null;
            }

            blackScreen.alpha = 1f;
        }

        SceneManager.LoadScene(sceneToLoadIndex);
    }
}