using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class NoHelmetViolationTrigger : MonoBehaviour
{
    private readonly HashSet<NoHelmetVehicleViolation> triggeredVehicles =
        new HashSet<NoHelmetVehicleViolation>();

    [Header("Warning Sound")]
    [Tooltip("AudioSource that plays the warning/alarm.")]
    [SerializeField] private AudioSource warningAudioSource;

    [Tooltip("Sound that plays while the motorist is in the violation zone.")]
    [SerializeField] private AudioClip warningSound;

    private NoHelmetVehicleViolation currentVehicle;

    private void Reset()
    {
        SetupTrigger();
    }

    private void Awake()
    {
        SetupTrigger();

        // Make sure it does not start playing automatically.
        if (warningAudioSource != null)
        {
            warningAudioSource.playOnAwake = false;
            warningAudioSource.loop = true;
            warningAudioSource.Stop();
        }
    }

    private void SetupTrigger()
    {
        Collider triggerCollider = GetComponent<Collider>();

        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
        }

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

        // Prevent the same motorist from activating the violation twice.
        if (triggeredVehicles.Contains(vehicleViolation))
            return;

        triggeredVehicles.Add(vehicleViolation);
        currentVehicle = vehicleViolation;

        // Start the alarm and keep looping it.
        StartWarningSound();

        // Start slowdown + red outline + interaction timer.
        vehicleViolation.BeginViolationWarning();

        Debug.Log("Motorist entered No Helmet Zone - alarm started.");
    }

    private void OnTriggerExit(Collider other)
    {
        NoHelmetVehicleViolation vehicleViolation =
            other.GetComponentInParent<NoHelmetVehicleViolation>();

        if (vehicleViolation == null)
            return;

        if (vehicleViolation == currentVehicle)
        {
            StopWarningSound();
            currentVehicle = null;
        }
    }

    private void StartWarningSound()
    {
        if (warningAudioSource == null || warningSound == null)
            return;

        warningAudioSource.clip = warningSound;
        warningAudioSource.loop = true;

        if (!warningAudioSource.isPlaying)
        {
            warningAudioSource.Play();
        }
    }

    // NPCChoiceInteraction calls this when the player stops the motorist.
    public void StopWarningSound()
    {
        if (warningAudioSource != null)
        {
            warningAudioSource.Stop();
        }

        Debug.Log("No Helmet alarm stopped.");
    }
}