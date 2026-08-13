using UnityEngine;

public class PlayerCarLookDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float lookDistance = 50f;
    [SerializeField] private LayerMask vehicleLayer;

    [Header("Target Vehicles")]
    [SerializeField] private GameObject carA;
    [SerializeField] private GameObject carB;

    [Header("Crash Manager")]
    [SerializeField] private CarCrashManager crashManager;

    [Header("Day Intro")]
    [Tooltip("Detection will stay disabled until EnableDetection() is called.")]
    [SerializeField] private bool waitForDayIntro = true;

    private bool detectionEnabled = false;
    private bool hasTriggered = false;

    private void Awake()
    {
        // If we don't need an intro delay,
        // detection can begin immediately.
        detectionEnabled = !waitForDayIntro;
    }

    private void Update()
    {
        // Do absolutely nothing while Day Intro is showing.
        if (!detectionEnabled)
            return;

        if (hasTriggered)
            return;

        Ray ray = new Ray(
            transform.position,
            transform.forward
        );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            lookDistance,
            vehicleLayer))
        {
            GameObject hitObject =
                hit.collider.gameObject;

            bool lookingAtCarA =
                carA != null &&
                (
                    hitObject == carA ||
                    hitObject.transform.IsChildOf(carA.transform)
                );

            bool lookingAtCarB =
                carB != null &&
                (
                    hitObject == carB ||
                    hitObject.transform.IsChildOf(carB.transform)
                );

            if (lookingAtCarA || lookingAtCarB)
            {
                hasTriggered = true;

                Debug.Log(
                    "Player is looking at crash cars. Triggering crash."
                );

                if (crashManager != null)
                {
                    crashManager.TriggerCrashSequence();
                }
                else
                {
                    Debug.LogError(
                        "PlayerCarLookDetector: Crash Manager is missing!"
                    );
                }
            }
        }
    }

    // Call this ONLY after the Day Intro has completely faded away.
    public void EnableDetection()
    {
        detectionEnabled = true;

        Debug.Log(
            "PlayerCarLookDetector: Car detection enabled."
        );
    }

    public void DisableDetection()
    {
        detectionEnabled = false;
    }
}