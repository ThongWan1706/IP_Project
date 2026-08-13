using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class NoHelmetViolationTrigger : MonoBehaviour
{
    private readonly HashSet<NoHelmetVehicleViolation> triggeredVehicles =
        new HashSet<NoHelmetVehicleViolation>();

    [Header("Warning Sound")]
    [Tooltip("AudioSource used to play the warning sound.")]
    [SerializeField] private AudioSource warningAudioSource;

    [Tooltip("Sound played once when a rider enters the no-helmet trigger zone.")]
    [SerializeField] private AudioClip warningSound;

    private void Reset()
    {
        Collider triggerCollider = GetComponent<Collider>();

        if (triggerCollider != null)
            triggerCollider.isTrigger = true;

        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    private void Awake()
    {
        Collider triggerCollider = GetComponent<Collider>();

        if (triggerCollider != null)
            triggerCollider.isTrigger = true;

        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        NoHelmetVehicleViolation vehicleViolation =
            other.GetComponentInParent<NoHelmetVehicleViolation>();

        if (vehicleViolation == null)
            return;

        // Only activate this trigger once for each violating vehicle.
        if (triggeredVehicles.Contains(vehicleViolation))
            return;

        triggeredVehicles.Add(vehicleViolation);

        // Play the warning sound once when this rider enters the trigger.
        if (warningAudioSource != null && warningSound != null)
        {
            warningAudioSource.PlayOneShot(warningSound);
        }

        vehicleViolation.BeginViolationWarning();
    }
}