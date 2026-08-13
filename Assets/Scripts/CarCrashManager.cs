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

    [Header("Scene Transition Settings")]
    [SerializeField] private float delayBeforeSceneLoad = 3f; // pause after crash
    [SerializeField] private int sceneToLoadIndex = 6; // scene to load after the fade

    [Header("Black Screen Fade")]
    [Tooltip("Assign a full-screen black UI Image with a CanvasGroup component.")]
    [SerializeField] private CanvasGroup blackScreen;

    [Tooltip("How long it takes to fade from the crash scene to black.")]
    [SerializeField] private float fadeDuration = 1.5f;

    private bool crashTriggered = false;

    private void Awake()
    {
        // Start with the black screen invisible.
        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);
            blackScreen.alpha = 0f;
            blackScreen.interactable = false;
            blackScreen.blocksRaycasts = false;
        }
    }

    public void TriggerCrashSequence()
    {
        if (crashTriggered) return;
        crashTriggered = true;

        if (carA == null || carB == null)
        {
            Debug.LogError("CarCrashManager: Assign Car A and Car B in the Inspector!");
            return;
        }

        PrepareCarForPhysics(carA, carB.transform.position);
        PrepareCarForPhysics(carB, carA.transform.position);

        // Wait after the crash, fade the screen to black,
        // then load the next scene.
        StartCoroutine(CrashThenFadeAndLoad());
    }

    private void PrepareCarForPhysics(GameObject car, Vector3 targetPosition)
    {
        if (car == null) return;

        CarNavMeshTraffic navTraffic = car.GetComponentInChildren<CarNavMeshTraffic>();
        if (navTraffic != null) navTraffic.enabled = false;

        NavMeshAgent agent = car.GetComponentInChildren<NavMeshAgent>();
        if (agent != null)
        {
            agent.isStopped = true;
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

        Vector3 driveDirection = targetPosition - car.transform.position;
        driveDirection.y = 0f;
        driveDirection.Normalize();

        Vector3 crashVelocity = (driveDirection * inwardForce) + (Vector3.up * upwardLiftForce);
        rb.AddForce(crashVelocity, ForceMode.VelocityChange);

        Vector3 randomTorque = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ) * rotationalTorque;

        rb.AddTorque(randomTorque, ForceMode.VelocityChange);
    }

    private IEnumerator CrashThenFadeAndLoad()
    {
        // Let the player see the crash first.
        yield return new WaitForSeconds(delayBeforeSceneLoad);

        if (blackScreen == null)
        {
            Debug.LogWarning(
                "CarCrashManager: Black Screen is not assigned. Loading scene without fade."
            );

            SceneManager.LoadScene(sceneToLoadIndex);
            yield break;
        }

        blackScreen.gameObject.SetActive(true);
        blackScreen.blocksRaycasts = true;
        blackScreen.interactable = true;

        float timer = 0f;
        float startAlpha = blackScreen.alpha;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            blackScreen.alpha = Mathf.Lerp(
                startAlpha,
                1f,
                timer / fadeDuration
            );

            yield return null;
        }

        blackScreen.alpha = 1f;

        Debug.Log($"Loading scene index {sceneToLoadIndex}...");
        SceneManager.LoadScene(sceneToLoadIndex);
    }
}