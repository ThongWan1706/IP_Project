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

    [Header("Crash Sound")]
    [SerializeField] private AudioSource crashAudioSource;
 
    private bool hasTriggered = false;

    private void Update()
    {
        if (hasTriggered) return;

        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, lookDistance, vehicleLayer))
        {
            GameObject hitObject = hit.collider.gameObject;

            // Detect car or any nested sub-mesh/collider child object
            if (hitObject == carA || hitObject == carB ||
                hitObject.transform.IsChildOf(carA.transform) ||
                hitObject.transform.IsChildOf(carB.transform))
            {
                hasTriggered = true;

                // Play crash sound
                if (crashAudioSource != null)
                {
                    crashAudioSource.Play();
                }
                else
                {
                    Debug.LogWarning(
                        "PlayerCarLookDetector: Crash Audio Source or Crash Sound is missing."
                    );
                }

                // Trigger the existing crash sequence
                if (crashManager != null)
                {
                    crashManager.TriggerCrashSequence();
                }
                else
                {
                    Debug.LogError(
                        "PlayerCarLookDetector: Crash Manager reference is missing!"
                    );
                }
            }
        }
    }
}